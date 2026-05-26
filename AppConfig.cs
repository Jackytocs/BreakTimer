using System;

namespace BreakTimer
{
    /// <summary>
    /// Represents the application configuration including work/break durations,
    /// theme preferences, and notification settings.
    /// </summary>
    public class AppConfig
    {
        #region Properties

        /// <summary>
        /// Duration of work session in minutes before a break is triggered.
        /// Default: 30 minutes. Range: 1-180 minutes.
        /// </summary>
        public int WorkDurationMinutes { get; set; } = 30;

        /// <summary>
        /// Duration of break time in minutes.
        /// Default: 1 minute. Range: 1-30 minutes.
        /// </summary>
        public int BreakDurationMinutes { get; set; } = 1;

        /// <summary>
        /// Indicates whether the application should start automatically with Windows.
        /// </summary>
        public bool StartWithWindows { get; set; } = false;

        /// <summary>
        /// Indicates whether sound notifications are enabled for break start/end.
        /// </summary>
        public bool SoundNotificationsEnabled { get; set; } = true;

        /// <summary>
        /// Selected UI theme: "dark" or "light".
        /// Default: "dark" for reduced eye strain during work hours.
        /// </summary>
        public string Theme { get; set; } = "dark";

        /// <summary>
        /// Indicates whether to show session statistics on break screen.
        /// </summary>
        public bool ShowStatisticsOnBreak { get; set; } = true;

        /// <summary>
        /// Indicates whether to show a confirmation dialog when skipping breaks.
        /// </summary>
        public bool ConfirmSkip { get; set; } = true;

        /// <summary>
        /// Background color for break screen (hex format, e.g., "#1A1A1A").
        /// </summary>
        public string BreakScreenBackgroundColor { get; set; } = "#141414";

        /// <summary>
        /// Text color for break screen (hex format, e.g., "#FFFFFF").
        /// </summary>
        public string BreakScreenTextColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// Accent color for motivational messages (hex format, e.g., "#87CEEB").
        /// </summary>
        public string AccentColor { get; set; } = "#87CEEB";

        #endregion

        #region Methods

        /// <summary>
        /// Validates the configuration values and adjusts invalid values to acceptable ranges.
        /// </summary>
        public void Validate()
        {
            // Validate work duration
            if (WorkDurationMinutes < 1) WorkDurationMinutes = 1;
            if (WorkDurationMinutes > 180) WorkDurationMinutes = 180;

            // Validate break duration
            if (BreakDurationMinutes < 1) BreakDurationMinutes = 1;
            if (BreakDurationMinutes > 30) BreakDurationMinutes = 30;

            // Validate theme
            if (string.IsNullOrEmpty(Theme) || (Theme != "dark" && Theme != "light"))
                Theme = "dark";
        }

        /// <summary>
        /// Creates a deep copy of the current configuration.
        /// </summary>
        /// <returns>A new AppConfig instance with the same values.</returns>
        public AppConfig Clone()
        {
            return new AppConfig
            {
                WorkDurationMinutes = this.WorkDurationMinutes,
                BreakDurationMinutes = this.BreakDurationMinutes,
                StartWithWindows = this.StartWithWindows,
                SoundNotificationsEnabled = this.SoundNotificationsEnabled,
                Theme = this.Theme,
                ShowStatisticsOnBreak = this.ShowStatisticsOnBreak,
                ConfirmSkip = this.ConfirmSkip,
                BreakScreenBackgroundColor = this.BreakScreenBackgroundColor,
                BreakScreenTextColor = this.BreakScreenTextColor,
                AccentColor = this.AccentColor
            };
        }

        #endregion
    }
}
