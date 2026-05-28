using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BreakTimer
{
    /// <summary>
    /// Enhanced break screen displayed when it's time for a break.
    /// Features:
    /// - Full-screen immersive experience with dark theme
    /// - Animated health messages (eye care, hydration, stretching, posture)
    /// - Session statistics display
    /// - Keyboard shortcuts (Space/Enter to skip, Esc to close)
    /// - Multi-monitor support with passive overlay shielding
    /// - Optional skip confirmation dialog
    /// </summary>
    public class BreakForm : Form
    {
        #region Fields

        private Label? timeLabel;
        private Label? motivationLabel;
        private Label? statisticsLabel;
        private Label? breakEndMessageLabel;
        private Button? skipButton;
        private System.Windows.Forms.Timer? countdownTimer;
        private System.Windows.Forms.Timer? fadeTimer;

        private int timeLeft;
        private int messageIndex = 0;
        private int cornerIndex = 0;
        private float fadeAlpha = 0f;
        private bool isFadingIn = true;
        private bool isClosing = false; // Flag to prevent multi-closure triggers

        private readonly int breakDurationMinutes;
        private readonly AppConfig config;
        private readonly SessionStatistics statistics;

        private string[]? breakMessages;

        // Container holding passive screen shields for multi-monitor setups
        private readonly List<SecondaryScreenForm> secondaryFades = new List<SecondaryScreenForm>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the break form with specified duration and configuration.
        /// </summary>
        /// <param name="breakDurationMinutes">Duration of break in minutes.</param>
        public BreakForm(int breakDurationMinutes)
        {
            this.breakDurationMinutes = breakDurationMinutes;
            this.timeLeft = breakDurationMinutes * 60;

            // Get config from Program's global ConfigManager
            config = Program.ConfigManager.Config;
            statistics = Program.ConfigManager.Statistics;

            // Get all break messages
            breakMessages = BreakMessages.GetAllMessages();

            InitializeForm();
            InitializeControls();
            SetupEventHandlers();
            ApplyTheme();
            DimSecondaryMonitors();

            // Play break start sound if enabled
            if (config.SoundNotificationsEnabled)
            {
                SoundManager.PlayBreakStartSound();
            }
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initializes form properties and positions it on primary monitor.
        /// </summary>
        private void InitializeForm()
        {
            this.Text = "Time for a Break!";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            // Position on primary monitor
            Screen primaryScreen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            this.Bounds = primaryScreen.Bounds;
        }

        /// <summary>
        /// Creates and initializes all UI controls.
        /// </summary>
        private void InitializeControls()
        {
            // Timer Display Label
            timeLabel = new Label()
            {
                Text = FormatTime(timeLeft),
                Font = new Font("Segoe UI", 48f, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(300, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Motivation/Health Message Label
            motivationLabel = new Label()
            {
                Text = breakMessages?[0] ?? "Take a break!",
                Font = new Font("Segoe UI", 28, FontStyle.Italic),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                MaximumSize = new Size(800, 200),
                BackColor = Color.Transparent
            };

            // Statistics Display Label
            statisticsLabel = new Label()
            {
                Text = statistics.GetSessionSummary(),
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(500, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = config.ShowStatisticsOnBreak
            };

            // Break End Message Label (shown at end)
            breakEndMessageLabel = new Label()
            {
                Text = BreakMessages.GetBreakEndMessage(),
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Skip Button
            skipButton = new Button()
            {
                Text = "Skip Break",
                Size = new Size(150, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            skipButton.FlatAppearance.BorderSize = 1;
            skipButton.Click += SkipButton_Click;

            // Add controls to form
            this.Controls.Add(timeLabel);
            this.Controls.Add(motivationLabel);
            this.Controls.Add(statisticsLabel);
            this.Controls.Add(breakEndMessageLabel);
            this.Controls.Add(skipButton);
        }

        /// <summary>
        /// Sets up timers and form event handlers.
        /// </summary>
        private void SetupEventHandlers()
        {
            // Main countdown timer (1 second tick)
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            // Fade animation timer (100ms for smooth transitions)
            fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 100;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();

            // Form layout event
            this.Load += (s, e) => { CenterControls(); MoveMessageToCorner(); };

            // Resize event for responsive layout
            this.Resize += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Maximized || this.Width == Screen.PrimaryScreen?.Bounds.Width)
                {
                    CenterControls();
                    MoveMessageToCorner();
                }
            };

            // Keyboard shortcuts
            this.KeyDown += BreakForm_KeyDown;
        }

        /// <summary>
        /// Applies theme colors from configuration to all controls.
        /// </summary>
        private void ApplyTheme()
        {
            try
            {
                // Parse colors from config (hex format)
                this.BackColor = ColorTranslator.FromHtml(config.BreakScreenBackgroundColor);
                this.ForeColor = ColorTranslator.FromHtml(config.BreakScreenTextColor);

                Color accentColor = ColorTranslator.FromHtml(config.AccentColor);
                Color textColor = ColorTranslator.FromHtml(config.BreakScreenTextColor);

                if (motivationLabel != null) motivationLabel.ForeColor = accentColor;
                if (statisticsLabel != null) statisticsLabel.ForeColor = textColor;
                if (breakEndMessageLabel != null) breakEndMessageLabel.ForeColor = accentColor;

                // Style skip button
                if (skipButton != null)
                {
                    skipButton.BackColor = this.BackColor;
                    skipButton.ForeColor = textColor;
                    skipButton.FlatAppearance.BorderColor = accentColor;
                }
            }
            catch
            {
                // Fallback to default theme if parsing fails
                this.BackColor = Color.FromArgb(20, 20, 20);
                this.ForeColor = Color.White;
                if (motivationLabel != null) motivationLabel.ForeColor = Color.LightSkyBlue;
            }
        }

        /// <summary>
        /// Instantiates lightweight background blocking shields over secondary active screens.
        /// Prevents users from bypassing breaks on multi-monitor setups.
        /// </summary>
        private void DimSecondaryMonitors()
        {
            try
            {
                foreach (var screen in Screen.AllScreens)
                {
                    if (!screen.Primary)
                    {
                        var shield = new SecondaryScreenForm(screen, this.BackColor);
                        shield.Show();
                        secondaryFades.Add(shield);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[BreakForm] Failed to dim secondary monitors: {ex}");
            }
        }

        #endregion

        #region Layout Methods

        /// <summary>
        /// Centers timer, button, and statistics labels on the screen.
        /// </summary>
        private void CenterControls()
        {
            if (timeLabel == null || statisticsLabel == null || skipButton == null)
                return;

            // Center timer at top-center
            timeLabel.Location = new Point(
                (this.Width - timeLabel.Width) / 2,
                this.Height / 4
            );

            // Center statistics below timer
            statisticsLabel.Location = new Point(
                (this.Width - statisticsLabel.Width) / 2,
                timeLabel.Bottom + 20
            );

            // Center skip button at bottom-center
            skipButton.Location = new Point(
                (this.Width - skipButton.Width) / 2,
                this.Height - 100
            );
        }

        /// <summary>
        /// Moves motivational message to one of the four corners in sequence.
        /// </summary>
        private void MoveMessageToCorner()
        {
            if (motivationLabel == null)
                return;

            int padding = 60;
            switch (cornerIndex)
            {
                case 0: // Top Left
                    motivationLabel.Location = new Point(padding, padding);
                    break;
                case 1: // Top Right
                    motivationLabel.Location = new Point(this.Width - motivationLabel.Width - padding, padding);
                    break;
                case 2: // Bottom Right
                    motivationLabel.Location = new Point(
                        this.Width - motivationLabel.Width - padding,
                        this.Height - motivationLabel.Height - padding
                    );
                    break;
                case 3: // Bottom Left
                    motivationLabel.Location = new Point(padding, this.Height - motivationLabel.Height - padding);
                    break;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles countdown timer tick - updates time display and messages.
        /// </summary>
        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (timeLabel == null || statisticsLabel == null || motivationLabel == null || breakMessages == null || isClosing)
                return;

            timeLeft--;
            timeLabel.Text = FormatTime(timeLeft);

            // Every 5 seconds: change message and move to next corner
            if (timeLeft % 5 == 0 && timeLeft > 0)
            {
                messageIndex = (messageIndex + 1) % breakMessages.Length;
                motivationLabel.Text = breakMessages[messageIndex];
                cornerIndex = (cornerIndex + 1) % 4;
                MoveMessageToCorner();

                // Reset fade animation for new message
                fadeAlpha = 0f;
                isFadingIn = true;
                UpdateMotivationLabelFade();
            }

            // Update statistics
            statisticsLabel.Text = statistics.GetSessionSummary();

            // Break time finished
            if (timeLeft <= 0)
            {
                BreakTimeComplete();
            }
        }

        /// <summary>
        /// Handles fade animation timer for smooth text transitions.
        /// </summary>
        private void FadeTimer_Tick(object? sender, EventArgs e)
        {
            if (isFadingIn)
            {
                fadeAlpha += 0.1f;
                if (fadeAlpha >= 1f)
                {
                    fadeAlpha = 1f;
                    isFadingIn = false;
                }

                UpdateMotivationLabelFade();
            }
        }

        /// <summary>
        /// Handles skip button click with optional confirmation dialog.
        /// </summary>
        private void SkipButton_Click(object? sender, EventArgs e)
        {
            if (config.ConfirmSkip)
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure? Your eyes and body need this rest!\n\nContinue the break?",
                    "Skip Break Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                );

                if (result != DialogResult.Yes)
                    return;
            }

            SoundManager.PlayBeep();
            CloseFormGracefully();
        }

        /// <summary>
        /// Handles keyboard shortcuts:
        /// Space/Enter = Skip, Esc = Close
        /// </summary>
        private void BreakForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                SkipButton_Click(null, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CloseFormGracefully();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when break time completes - records stats and closes form asynchronously.
        /// Uses async Task.Delay instead of Thread.Sleep to keep UI responsive.
        /// </summary>
        private async void BreakTimeComplete()
        {
            if (isClosing) return;
            isClosing = true;

            countdownTimer?.Stop();
            fadeTimer?.Stop();

            // Record the break in statistics
            statistics.RecordBreak(breakDurationMinutes * 60);
            Program.ConfigManager.SaveStatistics();

            // Play break end sound if enabled
            if (config.SoundNotificationsEnabled)
            {
                SoundManager.PlayBreakEndSound();
            }

            // Show completion message briefly
            if (motivationLabel != null) motivationLabel.Text = BreakMessages.GetBreakEndMessage();
            if (timeLabel != null) timeLabel.Text = "00:00";

            // Non-blocking UI delay ensures UI remains responsive
            await Task.Delay(2000);
            CloseFormGracefully();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Safely terminates the timers and closes the form.
        /// </summary>
        private void CloseFormGracefully()
        {
            isClosing = true;
            this.Close();
        }

        /// <summary>
        /// Applies the current fade alpha value to the motivational label.
        /// </summary>
        private void UpdateMotivationLabelFade()
        {
            if (motivationLabel == null)
                return;

            Color accentColor = ColorTranslator.FromHtml(config.AccentColor);
            int alpha = Math.Max(0, Math.Min(255, (int)(fadeAlpha * 255)));
            motivationLabel.ForeColor = Color.FromArgb(alpha, accentColor);
        }

        /// <summary>
        /// Formats seconds into MM:SS display format.
        /// </summary>
        /// <param name="seconds">Total seconds to format.</param>
        /// <returns>Formatted time string (MM:SS).</returns>
        private string FormatTime(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            return $"{minutes:D2}:{secs:D2}";
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Ensures timers are stopped and secondary screen overlays are disposed when form closes.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            countdownTimer?.Stop();
            countdownTimer?.Dispose();
            fadeTimer?.Stop();
            fadeTimer?.Dispose();

            // Safely close and dispose secondary screen overlays
            foreach (var form in secondaryFades)
            {
                try { form.Close(); form.Dispose(); } catch { }
            }
            secondaryFades.Clear();

            base.OnFormClosed(e);
        }

        #endregion
    }

    /// <summary>
    /// Highly lightweight, passive background window covering secondary desktop monitors during breaks.
    /// Prevents users from bypassing wellness breaks on multi-monitor setups.
    /// </summary>
    public class SecondaryScreenForm : Form
    {
        /// <summary>
        /// Initializes a secondary screen overlay with specified display bounds and background color.
        /// </summary>
        public SecondaryScreenForm(Screen screen, Color backColor)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = screen.Bounds;
            this.BackColor = backColor;
            this.TopMost = true;
            this.ShowInTaskbar = false;
        }

        /// <summary>
        /// Prevents the secondary overlay from taking input focus.
        /// </summary>
        protected override bool ShowWithoutActivation => true;
    }
}