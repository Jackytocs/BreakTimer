using System;
using System.Collections.Generic;
using System.Linq;

namespace BreakTimer
{
    /// <summary>
    /// Tracks session statistics including breaks taken, work time, and daily metrics
    /// to provide users with insights into their break and work habits.
    /// </summary>
    public class SessionStatistics
    {
        #region Properties

        /// <summary>
        /// Total number of breaks taken since the app started (current session).
        /// </summary>
        public int BreaksTaken { get; set; } = 0;

        /// <summary>
        /// Total work time elapsed in seconds since the timer started (current session).
        /// </summary>
        public long TotalWorkTimeSeconds { get; set; } = 0;

        /// <summary>
        /// Total break time consumed in seconds (current session).
        /// </summary>
        public long TotalBreakTimeSeconds { get; set; } = 0;

        /// <summary>
        /// Timestamp when the current session started.
        /// </summary>
        public DateTime SessionStartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Daily statistics tracked by date for historical tracking.
        /// Key: Date (YYYY-MM-DD), Value: Daily stats
        /// </summary>
        public Dictionary<string, DailyStatistics> DailyStats { get; set; } = new Dictionary<string, DailyStatistics>();

        #endregion

        #region Methods

        /// <summary>
        /// Records a break completion and updates all related statistics.
        /// </summary>
        /// <param name="breakDurationSeconds">Duration of the break in seconds.</param>
        public void RecordBreak(int breakDurationSeconds)
        {
            BreaksTaken++;
            TotalBreakTimeSeconds += breakDurationSeconds;

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (!DailyStats.ContainsKey(today))
            {
                DailyStats[today] = new DailyStatistics();
            }

            DailyStats[today].BreaksTaken++;
            DailyStats[today].TotalBreakTimeSeconds += breakDurationSeconds;
        }

        /// <summary>
        /// Updates total work time. Called periodically during work sessions.
        /// </summary>
        /// <param name="seconds">Number of seconds elapsed.</param>
        public void UpdateWorkTime(long seconds)
        {
            TotalWorkTimeSeconds += seconds;

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (!DailyStats.ContainsKey(today))
            {
                DailyStats[today] = new DailyStatistics();
            }

            DailyStats[today].TotalWorkTimeSeconds += seconds;
        }

        /// <summary>
        /// Gets formatted string of current session statistics for display.
        /// </summary>
        /// <returns>Human-readable statistics string.</returns>
        public string GetSessionSummary()
        {
            int workHours = (int)(TotalWorkTimeSeconds / 3600);
            int workMinutes = (int)((TotalWorkTimeSeconds % 3600) / 60);
            int breakMinutes = (int)(TotalBreakTimeSeconds / 60);

            return $"Today: {BreaksTaken} breaks | " +
                   $"{workHours}h {workMinutes}m worked | " +
                   $"{breakMinutes}m break time";
        }

        /// <summary>
        /// Gets daily statistics for a specific date.
        /// </summary>
        /// <param name="date">Date in YYYY-MM-DD format.</param>
        /// <returns>DailyStatistics for that date, or new empty stats if not found.</returns>
        public DailyStatistics GetDailyStats(string date)
        {
            return DailyStats.ContainsKey(date) ? DailyStats[date] : new DailyStatistics();
        }

        /// <summary>
        /// Calculates average breaks per day over the last 7 days.
        /// </summary>
        /// <returns>Average number of breaks per day.</returns>
        public double GetWeeklyAverageBreaks()
        {
            var last7Days = DailyStats
                .Where(kvp => DateTime.TryParse(kvp.Key, out var d) && d >= DateTime.Now.AddDays(-7))
                .ToList();

            if (last7Days.Count == 0) return 0;
            return last7Days.Average(kvp => kvp.Value.BreaksTaken);
        }

        /// <summary>
        /// Resets current session statistics while preserving historical data.
        /// </summary>
        public void ResetSession()
        {
            BreaksTaken = 0;
            TotalWorkTimeSeconds = 0;
            TotalBreakTimeSeconds = 0;
            SessionStartTime = DateTime.Now;
        }

        #endregion
    }

    /// <summary>
    /// Represents daily statistics for a single day.
    /// </summary>
    public class DailyStatistics
    {
        /// <summary>
        /// Number of breaks taken on this day.
        /// </summary>
        public int BreaksTaken { get; set; } = 0;

        /// <summary>
        /// Total work time in seconds on this day.
        /// </summary>
        public long TotalWorkTimeSeconds { get; set; } = 0;

        /// <summary>
        /// Total break time in seconds on this day.
        /// </summary>
        public long TotalBreakTimeSeconds { get; set; } = 0;

        /// <summary>
        /// Timestamp when this day's statistics were first recorded.
        /// </summary>
        public DateTime DateRecorded { get; set; } = DateTime.Now;
    }
}
