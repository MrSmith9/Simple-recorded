using System;

namespace SimpleRecord.Models
{
    /// <summary>
    /// Describes exactly what the next recording should capture: the whole
    /// screen, one window, or a custom area - plus whatever extra detail
    /// that choice needs (a window handle, or a pixel rectangle).
    /// </summary>
    public class RecordingSourceSelection
    {
        public RecordingSourceMode Mode { get; private init; } = RecordingSourceMode.FullScreen;

        public IntPtr WindowHandle { get; private init; }
        public string? WindowTitle { get; private init; }

        public int RegionX { get; private init; }
        public int RegionY { get; private init; }
        public int RegionWidth { get; private init; }
        public int RegionHeight { get; private init; }

        /// <summary>
        /// Which monitor to record, when Mode is FullScreen - Windows'
        /// own device name for it, e.g. "\\.\DISPLAY2" (see
        /// MonitorInfo.DeviceName). Null means "just use the primary
        /// monitor", which is also what happens on a single-monitor PC,
        /// where MainWindow never even shows the monitor-picker.
        /// </summary>
        public string? MonitorDeviceName { get; private init; }

        /// <summary>The short label for the chosen monitor (e.g. "Monitor 2"), for DisplayText below.</summary>
        public string? MonitorLabel { get; private init; }

        public static RecordingSourceSelection FullScreen() => new()
        {
            Mode = RecordingSourceMode.FullScreen
        };

        /// <summary>
        /// Whole-screen recording of one specific monitor, chosen from
        /// MonitorSelectDialog - used instead of FullScreen() whenever
        /// there's more than one monitor plugged in.
        /// </summary>
        public static RecordingSourceSelection ForMonitor(MonitorInfo monitor) => new()
        {
            Mode = RecordingSourceMode.FullScreen,
            MonitorDeviceName = monitor.DeviceName,
            MonitorLabel = $"Monitor {monitor.Index}"
        };

        public static RecordingSourceSelection ForWindow(WindowInfo window) => new()
        {
            Mode = RecordingSourceMode.Window,
            WindowHandle = window.Handle,
            WindowTitle = window.Title
        };

        public static RecordingSourceSelection ForRegion(int x, int y, int width, int height) => new()
        {
            Mode = RecordingSourceMode.Region,
            RegionX = x,
            RegionY = y,
            RegionWidth = width,
            RegionHeight = height
        };

        /// <summary>A short, human-readable description for the UI and the confirmation dialog.</summary>
        public string DisplayText => Mode switch
        {
            RecordingSourceMode.Window => $"Window - \"{WindowTitle}\"",
            RecordingSourceMode.Region => $"Custom area ({RegionWidth} x {RegionHeight} pixels)",
            _ => MonitorLabel != null ? $"Whole screen ({MonitorLabel})" : "Whole screen (primary monitor)"
        };
    }
}
