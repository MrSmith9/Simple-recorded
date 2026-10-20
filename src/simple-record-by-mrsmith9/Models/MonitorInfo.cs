namespace SimpleRecord.Models
{
    /// <summary>
    /// A lightweight, app-level stand-in for one monitor that's
    /// currently plugged in, so the UI and dialogs never need to know
    /// how Windows is actually asked for this information - see
    /// Services/MonitorService.cs for that.
    /// </summary>
    public class MonitorInfo
    {
        /// <summary>
        /// Windows' own internal name for this monitor, like
        /// "\\.\DISPLAY1" - this is exactly what ScreenRecorderLib needs
        /// to record this specific monitor (see
        /// RecordingService.BuildRecordingSources). Not something a
        /// person should ever have to read themselves - DisplayLabel
        /// below is what's actually shown in the picker.
        /// </summary>
        public string DeviceName { get; }

        public bool IsPrimary { get; }
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// This monitor's position in the list (1 = "Monitor 1", 2 =
        /// "Monitor 2", and so on). Filled in by MonitorService once the
        /// full list of monitors is known, since this numbering depends
        /// on where this monitor sits relative to the others, not on
        /// anything Windows reports about this one monitor by itself.
        /// Kept as a plain number (rather than only building it into
        /// DisplayLabel below) so other code can refer to "Monitor 2"
        /// without having to pull that back out of a sentence.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// A short, human-readable label, e.g. "Monitor 1 (Primary) -
        /// 1920 x 1080". Also filled in by MonitorService, from Index
        /// above plus this monitor's own details.
        /// </summary>
        public string DisplayLabel { get; set; } = string.Empty;

        public MonitorInfo(string deviceName, bool isPrimary, int width, int height)
        {
            DeviceName = deviceName;
            IsPrimary = isPrimary;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// WPF's ListBox shows this automatically for each item when no
        /// other display template is set, matching the same pattern
        /// WindowInfo uses for the window-picker list.
        /// </summary>
        public override string ToString() => DisplayLabel;
    }
}
