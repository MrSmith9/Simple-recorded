using System.Windows;
using System.Windows.Input;
using SimpleRecord.Models;
using SimpleRecord.Services;

namespace SimpleRecord.Views
{
    /// <summary>
    /// Lets the user pick one connected monitor to record, from the
    /// live list MonitorService builds. Only ever shown when there's
    /// more than one monitor plugged in - see
    /// MainWindow.FullScreenSourceButton_Click, which skips straight to
    /// recording when there's just one, since there'd be nothing to
    /// choose. Fully usable with just the keyboard (arrow keys + Enter),
    /// or with the mouse (click, or double-click to select immediately)
    /// - same pattern as WindowSelectDialog.
    /// </summary>
    public partial class MonitorSelectDialog : Window
    {
        public MonitorInfo? SelectedMonitor { get; private set; }

        public MonitorSelectDialog()
        {
            InitializeComponent();

            MonitorListBox.ItemsSource = MonitorService.GetMonitors();

            Loaded += (_, _) =>
            {
                if (MonitorListBox.Items.Count > 0)
                {
                    MonitorListBox.SelectedIndex = 0;
                }
                MonitorListBox.Focus();
            };
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            TryConfirmSelection();
        }

        private void MonitorListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TryConfirmSelection();
        }

        private void TryConfirmSelection()
        {
            if (MonitorListBox.SelectedItem is MonitorInfo info)
            {
                SelectedMonitor = info;
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
