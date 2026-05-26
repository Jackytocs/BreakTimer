using System;
using System.Collections.Generic;

namespace BreakTimer
{
    /// <summary>
    /// Manages customizable break messages and health-focused content.
    /// Includes eye care tips, hydration reminders, and stretching exercises.
    /// </summary>
    public static class BreakMessages
    {
        #region Fields

        /// <summary>
        /// Array of eye care messages to encourage proper eye movement during breaks.
        /// </summary>
        private static readonly string[] EyeCareTips = new string[]
        {
            "Look up here. Take a deep breath in...",
            "Now look over here. Exhale slowly...",
            "Down here. Rest your eyes. Blink a few times.",
            "And over here. Roll your shoulders back."
        };

        /// <summary>
        /// Array of hydration reminders encouraging water intake.
        /// </summary>
        private static readonly string[] HydrationReminders = new string[]
        {
            "💧 Time to hydrate! Grab a glass of water.",
            "💧 Your body needs water. Take a sip!",
            "💧 Stay hydrated for better focus and health.",
            "💧 Drink water to refresh your mind and body."
        };

        /// <summary>
        /// Array of stretching exercise instructions for physical wellness.
        /// </summary>
        private static readonly string[] StretchingExercises = new string[]
        {
            "🧘 Stretch your neck: Slowly turn your head left and right.",
            "🧘 Roll your shoulders back 5 times to release tension.",
            "🧘 Stand and stretch your arms up, touch your toes.",
            "🧘 Rotate your wrists and ankles to improve circulation.",
            "🧘 Do gentle neck rolls: 5 times forward, 5 times back."
        };

        /// <summary>
        /// Array of posture correction messages.
        /// </summary>
        private static readonly string[] PostureReminders = new string[]
        {
            "📍 Check your posture: Sit up straight!",
            "📍 Align your shoulders with your hips.",
            "📍 Keep your screen at eye level.",
            "📍 Feet flat on floor. Back against chair."
        };

        /// <summary>
        /// Array of general motivational messages.
        /// </summary>
        private static readonly string[] MotivationalMessages = new string[]
        {
            "✨ Great job staying focused! You're doing amazing!",
            "✨ Your health matters. Keep taking breaks!",
            "✨ Rest well. You've earned this break!",
            "✨ Taking breaks improves productivity. Good work!"
        };

        #endregion

        #region Methods

        /// <summary>
        /// Gets a rotating sequence of all break messages combining health tips and motivation.
        /// </summary>
        /// <returns>Array of break messages in a healthy sequence.</returns>
        public static string[] GetAllMessages()
        {
            var messages = new List<string>();
            messages.AddRange(EyeCareTips);
            messages.AddRange(HydrationReminders);
            messages.AddRange(StretchingExercises);
            messages.AddRange(PostureReminders);
            messages.AddRange(MotivationalMessages);
            return messages.ToArray();
        }

        /// <summary>
        /// Gets only eye care related messages.
        /// </summary>
        public static string[] GetEyeCareTips() => EyeCareTips;

        /// <summary>
        /// Gets only hydration reminder messages.
        /// </summary>
        public static string[] GetHydrationReminders() => HydrationReminders;

        /// <summary>
        /// Gets only stretching exercise messages.
        /// </summary>
        public static string[] GetStretchingExercises() => StretchingExercises;

        /// <summary>
        /// Gets only posture correction messages.
        /// </summary>
        public static string[] GetPostureReminders() => PostureReminders;

        /// <summary>
        /// Gets a random message from all available messages.
        /// Useful for variety during breaks.
        /// </summary>
        /// <returns>A random break message.</returns>
        public static string GetRandomMessage()
        {
            var allMessages = GetAllMessages();
            return allMessages[new Random().Next(allMessages.Length)];
        }

        /// <summary>
        /// Gets a message for the end of a break session.
        /// </summary>
        /// <returns>End-of-break motivational message.</returns>
        public static string GetBreakEndMessage()
        {
            string[] endMessages = new string[]
            {
                "✨ Break time's over! Ready to tackle more?",
                "✨ Refreshed and recharged. Let's go!",
                "✨ You've got this! Back to focus mode.",
                "✨ Energy restored. Time to be productive!"
            };
            return endMessages[new Random().Next(endMessages.Length)];
        }

        #endregion
    }
}
