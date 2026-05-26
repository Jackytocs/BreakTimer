using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BreakTimer
{
    /// <summary>
    /// Main settings and control window for BreakTimer application.
    /// Allows users to configure work/break durations, theme, notifications,
    /// and displays session statistics.
    /// </summary>
    public class SettingsForm : Form
    {
        #region Fields

        private NumericUpDown? workInput;
        private NumericUpDown? breakInput;
        private ComboBox? themeComboBox;
        private CheckBox? soundCheckBox;
        private CheckBox? confirmSkipCheckBox;
        private CheckBox? statsCheckBox;
        private Button? startButton;
        private Button? stopButton;
        private Button? testButton;
        private Button? helpButton;
        private CheckBox? startupCheckBox;
        private Label? statisticsDisplayLabel;
        private Button? resetStatsButton;
        
        // High-precision timer (1 second interval) with DateTime tracking
        private System.Windows.Forms.Timer? preciseTimer;
        private DateTime? workEndTime;
        private int breakMinutes;

        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the settings form with all configuration controls and displays.
        /// </summary>
        public SettingsForm()
        {
            this.Text = "Break Timer Settings";
            this.Size = new Size(700, 750);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Information;

            InitializeControls();
            LoadSettingsToForm();
            SetupEventHandlers();
            SetupTrayIcon();

            this.Resize += SettingsForm_Resize;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates and initializes all UI controls for the settings form.
        /// </summary>
        private void InitializeControls()
        {
            int yPosition = 25;
            const int lineHeight = 50;

            // Work Duration
            Label workLabel = new Label() { Text = "Work Duration (Minutes):", Left = 20, Top = yPosition, Width = 180 };
            workInput = new NumericUpDown() { Left = 210, Top = yPosition - 2, Width = 80, Minimum = 1, Maximum = 180, Value = 30 };
            this.Controls.Add(workLabel);
            this.Controls.Add(workInput);
            yPosition += lineHeight;

            // Break Duration
            Label breakLabel = new Label() { Text = "Break Duration (Minutes):", Left = 20, Top = yPosition, Width = 180 };
            breakInput = new NumericUpDown() { Left = 210, Top = yPosition - 2, Width = 80, Minimum = 1, Maximum = 30, Value = 1 };
            this.Controls.Add(breakLabel);
            this.Controls.Add(breakInput);
            yPosition += lineHeight;

            // Theme Selection
            Label themeLabel = new Label() { Text = "Break Screen Theme:", Left = 20, Top = yPosition, Width = 180 };
            themeComboBox = new ComboBox() { Left = 210, Top = yPosition - 2, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            themeComboBox.Items.AddRange(new string[] { "Dark (Default)", "Light" });
            themeComboBox.SelectedIndex = 0;
            this.Controls.Add(themeLabel);
            this.Controls.Add(themeComboBox);
            yPosition += lineHeight;

            // Checkboxes
            soundCheckBox = new CheckBox() { Text = "Enable sound notifications", Left = 20, Top = yPosition, AutoSize = true, Checked = true };
            this.Controls.Add(soundCheckBox);
            yPosition += 35;

            confirmSkipCheckBox = new CheckBox() { Text = "Confirm before skipping breaks", Left = 20, Top = yPosition, AutoSize = true, Checked = true };
            this.Controls.Add(confirmSkipCheckBox);
            yPosition += 35;

            statsCheckBox = new CheckBox() { Text = "Show statistics during breaks", Left = 20, Top = yPosition, AutoSize = true, Checked = true };
            this.Controls.Add(statsCheckBox);
            yPosition += 35;

            startupCheckBox = new CheckBox() { Text = "Start automatically with Windows", Left = 20, Top = yPosition, AutoSize = true };
            startupCheckBox.Checked = CheckIfStartupEnabled();
            startupCheckBox.CheckedChanged += StartupCheckBox_CheckedChanged;
            this.Controls.Add(startupCheckBox);
            yPosition += 45;

            // Statistics display
            Label statsLabelTitle = new Label() { Text = "Today's Statistics:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Left = 20, Top = yPosition, Width = 500 };
            this.Controls.Add(statsLabelTitle);
            yPosition += 35;

            statisticsDisplayLabel = new Label()
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 9),
                Left = 30,
                Top = yPosition,
                AutoSize = true,
                MaximumSize = new Size(640, 0)
            };
            this.Controls.Add(statisticsDisplayLabel);
            yPosition += 70;

            // Buttons
            startButton = new Button() { Text = "▶ Start", Left = 20, Top = yPosition, Width = 110, Height = 40, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);

            stopButton = new Button() { Text = "⏹ Stop", Left = 140, Top = yPosition, Width = 110, Height = 40, Font = new Font("Segoe UI", 10, FontStyle.Bold), Enabled = false };
            stopButton.Click += StopButton_Click;
            this.Controls.Add(stopButton);

            testButton = new Button() { Text = "🧪 Test Break", Left = 260, Top = yPosition, Width = 110, Height = 40, Font = new Font("Segoe UI", 10) };
            testButton.Click += TestButton_Click;
            this.Controls.Add(testButton);

            helpButton = new Button() { Text = "❓ Help", Left = 380, Top = yPosition, Width = 100, Height = 40, Font = new Font("Segoe UI", 10) };
            helpButton.Click += HelpButton_Click;
            this.Controls.Add(helpButton);

            resetStatsButton = new Button() { Text = "↻ Reset Stats", Left = 490, Top = yPosition, Width = 120, Height = 40, Font = new Font("Segoe UI", 10) };
            resetStatsButton.Click += ResetStatsButton_Click;
            this.Controls.Add(resetStatsButton);

            // Precise 1-second interval timer for accurate work countdown
            preciseTimer = new System.Windows.Forms.Timer();
            preciseTimer.Interval = 1000;
            preciseTimer.Tick += PreciseTimer_Tick;
        }

        /// <summary>
        /// Loads current configuration settings into the form controls.
        /// </summary>
        private void LoadSettingsToForm()
        {
            AppConfig config = Program.ConfigManager.Config;

            if (workInput != null) workInput.Value = config.WorkDurationMinutes;
            if (breakInput != null) breakInput.Value = config.BreakDurationMinutes;
            if (soundCheckBox != null) soundCheckBox.Checked = config.SoundNotificationsEnabled;
            if (confirmSkipCheckBox != null) confirmSkipCheckBox.Checked = config.ConfirmSkip;
            if (statsCheckBox != null) statsCheckBox.Checked = config.ShowStatisticsOnBreak;
            if (themeComboBox != null) themeComboBox.SelectedIndex = config.Theme == "light" ? 1 : 0;

            UpdateStatisticsDisplay();
        }

        /// <summary>
        /// Sets up form event handlers.
        /// </summary>
        private void SetupEventHandlers()
        {
            this.FormClosing += (s, e) => SaveSettingsFromForm();
        }

        /// <summary>
        /// Saves current form settings to configuration.
        /// </summary>
        private void SaveSettingsFromForm()
        {
            AppConfig config = Program.ConfigManager.Config;

            if (workInput != null) config.WorkDurationMinutes = (int)workInput.Value;
            if (breakInput != null) config.BreakDurationMinutes = (int)breakInput.Value;
            if (soundCheckBox != null) config.SoundNotificationsEnabled = soundCheckBox.Checked;
            if (confirmSkipCheckBox != null) config.ConfirmSkip = confirmSkipCheckBox.Checked;
            if (statsCheckBox != null) config.ShowStatisticsOnBreak = statsCheckBox.Checked;
            if (themeComboBox != null) config.Theme = themeComboBox.SelectedIndex == 1 ? "light" : "dark";

            Program.ConfigManager.SaveConfiguration();
        }

        #endregion

        #region Tray Icon Management

        /// <summary>
        /// Initializes the system tray icon with context menu.
        /// </summary>
        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Settings", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Break Timer (Stopped)";
            trayIcon.Icon = SystemIcons.Information;
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
        }

        #endregion

        #region Statistics Management

        /// <summary>
        /// Updates the statistics display label with current session stats.
        /// </summary>
        private void UpdateStatisticsDisplay()
        {
            SessionStatistics stats = Program.ConfigManager.Statistics;
            string display = stats.GetSessionSummary() +
                           $"\nWeekly Avg: {stats.GetWeeklyAverageBreaks():F1} breaks/day";
            if (statisticsDisplayLabel != null)
                statisticsDisplayLabel.Text = display;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles minimize button click - hides to tray.
        /// </summary>
        private void SettingsForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                if (trayIcon != null)
                    trayIcon.ShowBalloonTip(2000, "Break Timer", "App minimized to tray. Double-click tray icon to restore.", ToolTipIcon.Info);
            }
        }

        /// <summary>
        /// Handles Start button - begins work timer with precise DateTime tracking.
        /// </summary>
        private void StartButton_Click(object? sender, EventArgs e)
        {
            SaveSettingsFromForm();

            if (workInput == null || breakInput == null || startButton == null || stopButton == null || trayIcon == null || preciseTimer == null)
                return;

            int workMinutes = (int)workInput.Value;
            breakMinutes = (int)breakInput.Value;

            startButton.Enabled = false;
            stopButton.Enabled = true;

            this.Hide();

            // Set target ending time point (DateTime-based for precision)
            workEndTime = DateTime.Now.AddMinutes(workMinutes);
            preciseTimer.Start();

            trayIcon.ShowBalloonTip(3000, "Timer Started", $"Focus window active: {workMinutes} minutes.", ToolTipIcon.Info);
        }

        /// <summary>
        /// Handles Stop button - stops the running timer.
        /// </summary>
        private void StopButton_Click(object? sender, EventArgs e)
        {
            if (preciseTimer != null && startButton != null && stopButton != null && trayIcon != null)
            {
                preciseTimer.Stop();
                workEndTime = null;
                startButton.Enabled = true;
                stopButton.Enabled = false;
                trayIcon.Text = "Break Timer (Stopped)";
                MessageBox.Show("Timer stopped.", "Break Timer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Precise timer tick - tracks remaining work time using DateTime delta.
        /// Handles system sleep/suspend by comparing against actual wall clock time.
        /// </summary>
        private void PreciseTimer_Tick(object? sender, EventArgs e)
        {
            if (!workEndTime.HasValue || trayIcon == null || preciseTimer == null)
                return;

            TimeSpan timeLeft = workEndTime.Value - DateTime.Now;

            // Log continuous work seconds
            Program.ConfigManager.Statistics.UpdateWorkTime(1);

            if (timeLeft <= TimeSpan.Zero)
            {
                preciseTimer.Stop();
                workEndTime = null;
                TriggerBreak();
            }
            else
            {
                // Update System Tray Tooltip with real precise countdown status
                string progressStr = $"Break Timer - {timeLeft.Minutes:D2}m {timeLeft.Seconds:D2}s remaining";
                trayIcon.Text = progressStr.Length > 63 ? progressStr.Substring(0, 60) + "..." : progressStr;
            }
        }

        /// <summary>
        /// Triggers the break screen and auto-restarts if still running.
        /// </summary>
        private void TriggerBreak()
        {
            BreakForm breakScreen = new BreakForm(breakMinutes);
            breakScreen.ShowDialog();

            UpdateStatisticsDisplay();

            // Auto-restart work session cleanly if not manual stopped
            if (startButton != null && !startButton.Enabled && workInput != null && preciseTimer != null)
            {
                workEndTime = DateTime.Now.AddMinutes((double)workInput.Value);
                preciseTimer.Start();
            }
        }

        /// <summary>
        /// Handles Test Button - shows a 1-minute test break.
        /// </summary>
        private void TestButton_Click(object? sender, EventArgs e)
        {
            SaveSettingsFromForm();
            BreakForm testScreen = new BreakForm(1);
            testScreen.ShowDialog();
            UpdateStatisticsDisplay();
        }

        /// <summary>
        /// Handles Help button - shows help dialog.
        /// </summary>
        private void HelpButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "⏱️ Break Timer Help\n\n" +
                "• Set work and break durations\n" +
                "• Click Start to begin countdown\n" +
                "• During breaks, health tips appear\n" +
                "• Space/Enter to skip, Esc to close\n" +
                "• Statistics track your break habits\n" +
                "• Customize notifications and theme\n\n" +
                "Remember: Regular breaks improve focus and health! 💪",
                "Break Timer - Help",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Handles Reset Stats button - clears session statistics.
        /// </summary>
        private void ResetStatsButton_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Reset today's statistics?",
                "Confirm Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Program.ConfigManager.ResetSessionStatistics();
                UpdateStatisticsDisplay();
            }
        }

        /// <summary>
        /// Handles Windows Startup checkbox - enables/disables auto-start with proper path quoting.
        /// </summary>
        private void StartupCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (startupCheckBox == null)
                return;

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (key != null)
                {
                    if (startupCheckBox.Checked)
                    {
                        // Safely wrap execution executable paths in quotes to prevent path hijacking
                        string executionPath = $"\"{Environment.ProcessPath}\"";
                        key.SetValue("BreakTimer", executionPath);
                    }
                    else
                    {
                        key.DeleteValue("BreakTimer", false);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Failed to register startup path: {ex.Message}", "Security Restrictions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Registry Helpers

        /// <summary>
        /// Checks if BreakTimer is registered to start with Windows.
        /// </summary>
        /// <returns>True if startup is enabled, false otherwise.</returns>
        private bool CheckIfStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);

                return key?.GetValue("BreakTimer") != null;
            }
            catch { return false; }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Ensures all resources are properly disposed when form closes.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveSettingsFromForm();
            preciseTimer?.Stop();
            preciseTimer?.Dispose();
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }
            base.OnFormClosed(e);
        }

        #endregion
    }
}