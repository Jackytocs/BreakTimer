using System;
using System.IO;
using Newtonsoft.Json;

namespace BreakTimer
{
    /// <summary>
    /// Manages persistence of application configuration and session statistics to JSON files.
    /// Handles loading, saving, and default configuration initialization.
    /// Uses atomic file operations to prevent data corruption on system failures.
    /// </summary>
    public class ConfigManager
    {
        #region Fields

        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BreakTimer"
        );

        private static readonly string ConfigFilePath = Path.Combine(AppDataPath, "config.json");
        private static readonly string StatsFilePath = Path.Combine(AppDataPath, "statistics.json");

        private AppConfig _config = new AppConfig();
        private SessionStatistics _statistics = new SessionStatistics();

        // Event for notifying UI about warnings without tight coupling
        public event Action<string, Exception?>? ErrorOccurred;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current application configuration.
        /// </summary>
        public AppConfig Config => _config;

        /// <summary>
        /// Gets the current session statistics.
        /// </summary>
        public SessionStatistics Statistics => _statistics;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes ConfigManager and loads persisted settings and statistics.
        /// Creates default configuration if files don't exist.
        /// </summary>
        public ConfigManager()
        {
            EnsureAppDataDirectory();
            LoadConfiguration();
            LoadStatistics();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the current configuration to disk using atomic file replacement.
        /// Prevents data corruption by writing to a temporary file first.
        /// </summary>
        public void SaveConfiguration()
        {
            try
            {
                _config.Validate();
                string json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                SaveAtomically(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to save configuration settings atomically", ex);
            }
        }

        /// <summary>
        /// Saves the current session statistics to disk using atomic file replacement.
        /// </summary>
        public void SaveStatistics()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_statistics, Formatting.Indented);
                SaveAtomically(StatsFilePath, json);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to save statistics atomically", ex);
            }
        }

        /// <summary>
        /// Resets statistics for the current session while preserving historical data.
        /// </summary>
        public void ResetSessionStatistics()
        {
            _statistics.ResetSession();
            SaveStatistics();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures the AppData directory exists for BreakTimer configuration files.
        /// </summary>
        private void EnsureAppDataDirectory()
        {
            try
            {
                if (!Directory.Exists(AppDataPath))
                {
                    Directory.CreateDirectory(AppDataPath);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Could not create local data directory", ex);
            }
        }

        /// <summary>
        /// Prevents file corruption by writing to a temp file, then performing an atomic replace.
        /// This ensures that if the system crashes during a write, the original file remains intact.
        /// </summary>
        private void SaveAtomically(string filePath, string content)
        {
            string tempPath = filePath + ".tmp";
            File.WriteAllText(tempPath, content);

            try
            {
                // Atomic overwrite supported natively on modern Windows operating systems
                File.Move(tempPath, filePath, overwrite: true);
            }
            catch
            {
                // Safe structural fallback if Move fails
                if (File.Exists(filePath)) File.Delete(filePath);
                File.Move(tempPath, filePath);
            }
        }

        /// <summary>
        /// Loads configuration from disk, or creates default if file doesn't exist.
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    _config = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
                }
                else
                {
                    _config = new AppConfig();
                    SaveConfiguration();
                }

                _config.Validate();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Error loading configuration. Default settings applied.", ex);
                _config = new AppConfig();
            }
        }

        /// <summary>
        /// Loads statistics from disk, or creates default if file doesn't exist.
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                if (File.Exists(StatsFilePath))
                {
                    string json = File.ReadAllText(StatsFilePath);
                    _statistics = JsonConvert.DeserializeObject<SessionStatistics>(json) ?? new SessionStatistics();
                }
                else
                {
                    _statistics = new SessionStatistics();
                    SaveStatistics();
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Error loading statistics. Starting fresh session.", ex);
                _statistics = new SessionStatistics();
            }
        }

        #endregion
    }
}
