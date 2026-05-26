using System;
using System.IO;
using System.Media;

namespace BreakTimer
{
    /// <summary>
    /// Manages audio notifications for break start and end events.
    /// Provides gentle, non-intrusive sound cues to encourage break time participation.
    /// </summary>
    public class SoundManager
    {
        #region Fields

        private static readonly string SoundDirectory = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "",
            "Sounds"
        );

        #endregion

        #region Methods

        /// <summary>
        /// Plays a gentle sound to notify user that break time has started.
        /// Uses Windows system sounds for a subtle, professional notification.
        /// </summary>
        public static void PlayBreakStartSound()
        {
            try
            {
                // Use the system "Ding" sound - a common notification sound
                SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                // Silently fail - don't interrupt user experience if sound fails
                System.Diagnostics.Debug.WriteLine($"Sound play error: {ex.Message}");
            }
        }

        /// <summary>
        /// Plays a gentle sound to notify user that break time has ended.
        /// Uses a slightly different Windows system sound for distinction.
        /// </summary>
        public static void PlayBreakEndSound()
        {
            try
            {
                // Use the system "Ding" sound twice for break end
                SystemSounds.Asterisk.Play();
                System.Threading.Thread.Sleep(200);
                SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                // Silently fail
                System.Diagnostics.Debug.WriteLine($"Sound play error: {ex.Message}");
            }
        }

        /// <summary>
        /// Plays a gentle beep sound for UI interactions.
        /// </summary>
        public static void PlayBeep()
        {
            try
            {
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sound play error: {ex.Message}");
            }
        }

        /// <summary>
        /// Attempts to play a custom sound file if it exists.
        /// Falls back to system sounds if custom file is not found.
        /// </summary>
        /// <param name="soundFileName">Name of sound file in the Sounds directory.</param>
        public static void PlayCustomSound(string soundFileName)
        {
            try
            {
                string soundPath = Path.Combine(SoundDirectory, soundFileName);
                if (File.Exists(soundPath))
                {
                    var soundPlayer = new SoundPlayer(soundPath);
                    soundPlayer.PlaySync();
                }
                else
                {
                    // Fallback to system sound
                    SystemSounds.Asterisk.Play();
                }
            }
            catch (Exception ex)
            {
                // Silently fail
                System.Diagnostics.Debug.WriteLine($"Sound play error: {ex.Message}");
            }
        }

        #endregion
    }
}
