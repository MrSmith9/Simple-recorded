using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SimpleRecord.Models;

namespace SimpleRecord.Services
{
    /// <summary>
    /// Finds out which monitors are currently plugged in. Uses Windows'
    /// own low-level multi-monitor API directly (the same kind of plain
    /// Win32 P/Invoke call ScreenshotService uses for the screen size)
    /// rather than adding a reference to System.Windows.Forms just for
    /// this one piece of information.
    ///
    /// Used by MainWindow to decide whether clicking "Whole Screen"
    /// needs to ask which monitor at all (only when there's more than
    /// one), and by MonitorSelectDialog to fill in that list when it
    /// does.
    /// </summary>
    public static class MonitorService
    {
        // These three match the exact shape Windows itself uses for
        // this information (RECT and MONITORINFOEX, from winuser.h) -
        // the field names and sizes below aren't arbitrary, they have to
        // line up with what GetMonitorInfo actually writes into memory.
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct MONITORINFOEX
        {
            public int Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;

            // CCHDEVICENAME in the Windows headers is 32 - the fixed
            // buffer size Windows always uses for a monitor's device
            // name (things like "\\.\DISPLAY1").
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }

        private const uint MONITORINFOF_PRIMARY = 0x1;

        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        /// <summary>
        /// Lists every monitor Windows currently knows about, with the
        /// primary monitor always listed first, then the rest ordered
        /// left-to-right by position - matching how most people think of
        /// "Monitor 1, Monitor 2..." when they look at their actual
        /// desk. DisplayLabel is filled in here, once the full list (and
        /// therefore the numbering) is known.
        /// </summary>
        public static List<MonitorInfo> GetMonitors()
        {
            var found = new List<(string DeviceName, bool IsPrimary, int X, int Width, int Height)>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr data) =>
            {
                var info = new MONITORINFOEX { Size = Marshal.SizeOf(typeof(MONITORINFOEX)) };
                if (GetMonitorInfo(hMonitor, ref info))
                {
                    found.Add((
                        info.DeviceName,
                        (info.Flags & MONITORINFOF_PRIMARY) != 0,
                        info.Monitor.Left,
                        info.Monitor.Right - info.Monitor.Left,
                        info.Monitor.Bottom - info.Monitor.Top));
                }
                // Returning true tells Windows to keep going and report
                // the next monitor too.
                return true;
            }, IntPtr.Zero);

            List<(string DeviceName, bool IsPrimary, int X, int Width, int Height)> ordered = found
                .OrderByDescending(m => m.IsPrimary)
                .ThenBy(m => m.X)
                .ToList();

            var monitors = new List<MonitorInfo>();
            for (int i = 0; i < ordered.Count; i++)
            {
                (string DeviceName, bool IsPrimary, int X, int Width, int Height) m = ordered[i];
                int index = i + 1;
                string label = $"Monitor {index}{(m.IsPrimary ? " (Primary)" : "")} - {m.Width} x {m.Height}";
                monitors.Add(new MonitorInfo(m.DeviceName, m.IsPrimary, m.Width, m.Height)
                {
                    Index = index,
                    DisplayLabel = label
                });
            }

            return monitors;
        }
    }
}
