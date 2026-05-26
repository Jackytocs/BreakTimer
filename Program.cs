using System;
using System.IO;
using System.Windows.Forms;

namespace BreakTimer
{
    /// <summary>
    /// Entry point for the BreakTimer application.
    /// Initializes configuration, sets up global exception handling, and launches the settings window.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Global configuration manager instance shared across the application.
        /// </summary>
        public static ConfigManager ConfigManager { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            // Set up Global Exception Handlers before UI initializes
            Application.ThreadException += (sender, e) => LogFatalException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => LogFatalException(e.ExceptionObject as Exception);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize configuration and wire up error event handling
            ConfigManager = new ConfigManager();

            // Bind Configuration errors to non-disruptive notifications
            ConfigManager.ErrorOccurred += (message, ex) =>
            {
                System.Diagnostics.Trace.WriteLine($"[ConfigError] {message}: {ex?.Message}");
                MessageBox.Show($"{message}\n\nDetails: {ex?.Message}", "App Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            // Launch the settings window
            Application.Run(new SettingsForm());
        }

        /// <summary>
        /// Logs fatal exceptions to a crash log file and displays user-friendly error message.
        /// </summary>
        private static void LogFatalException(Exception? ex)
        {
            if (ex == null) return;

            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BreakTimer");
            string logFile = Path.Combine(logDir, "crash_log.txt");

            try
            {
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
                string errText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] FATAL EXCEPTION:\n{ex.Message}\n{ex.StackTrace}\n\n";
                File.AppendAllText(logFile, errText);
            }
            catch { /* Avoid nested crashes if disk writing fails */ }

            MessageBox.Show(
                "A fatal error occurred. The application will close.\n\n" +
                $"Details written to: {logFile}\n\nError: {ex.Message}",
                "Critical Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            
            Application.Exit();
        }
    }
}