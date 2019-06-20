using SessionPresenceUpdater.Providers;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace SessionPresenceUpdater {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private DiscordRpc.RichPresence _presence;
        private Timer _timer;
        private string _sessionName;

        DiscordRpc.EventHandlers handlers;


        public MainWindow() {
            InitializeComponent();
        }

        private void OnInitializeClicked(object sender, RoutedEventArgs e) {
            string clientId = TextBox_clientId.Text;
            bool isNumeric = ulong.TryParse(clientId, out ulong n);

            if (!isNumeric) {
                MessageBox.Show("The client ID must be a numeric value.");

                return;
            }

            // Initialize the Discord RPC.
            Initialize(clientId);

            Button_Start.IsEnabled = true;
            Button_Stop.IsEnabled = false;
            Button_Initialize.IsEnabled = false;
            _timer = new Timer();
            _timer.Interval = 30000;
            _timer.Elapsed += new ElapsedEventHandler(TimerTick);
        }

        private void OnStartClicked(object sender, RoutedEventArgs e) {

            if (string.IsNullOrEmpty(TextBox_sessionName.Text)) {
                MessageBox.Show("The session name field is required. Please enter a session name.");
            } else {
                Button_Start.IsEnabled = false;
                Button_Stop.IsEnabled = true;

                _sessionName = TextBox_sessionName.Text;

                DiscordRpc.RunCallbacks();


                // Start the timer for sending the HTTP requests.
                _timer.Start();

            }
        }

        private void OnStopClicked(object sender, RoutedEventArgs e) {
            Button_Start.IsEnabled = true;
            Button_Stop.IsEnabled = false;

            // Stop the timer for sending the HTTP requests.
            _timer.Stop();
        }


        /// <summary>
        /// Initialize the RPC.
        /// </summary>
        /// <param name="clientId"></param>
        private void Initialize(string clientId) {
            handlers = new DiscordRpc.EventHandlers();

            handlers.readyCallback = ReadyCallback;
            handlers.disconnectedCallback += DisconnectedCallback;
            handlers.errorCallback += ErrorCallback;

            DiscordRpc.Initialize(clientId, ref handlers, true, null);

            SetStatusBarMessage("Initialized.");
        }


        /// <summary>
        /// Called after RunCallbacks() when ready.
        /// </summary>
        private void ReadyCallback() {
            SetStatusBarMessage("Ready.");
        }


        /// <summary>
        /// Called after RunCallbacks() in cause of disconnection.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        private void DisconnectedCallback(int errorCode, string message) {
            SetStatusBarMessage(string.Format("Disconnect {0}: {1}", errorCode, message));
        }


        /// <summary>
        /// Called after RunCallbacks() in cause of error.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        private void ErrorCallback(int errorCode, string message) {
            SetStatusBarMessage(string.Format("Error {0}: {1}", errorCode, message));
        }


        /// <summary>
        /// Just set a message to be displayed in the status bar at the window's bottom.
        /// </summary>
        /// <param name="message"></param>
        private void SetStatusBarMessage(string message) {
            Label_Status.Content = message;
        }


        /// <summary>
        /// Fired when the timer period elapses.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TimerTick(object sender, EventArgs e) {
            SessionDetailProvider provider;
            SessionDetail data = null;
            string largeImageKey;
            string largeImageText;
            string smallImageKey;
            string smallImageText;


            provider = new SessionDetailProvider();
            largeImageKey = null;
            largeImageText = null;
            smallImageKey = null;
            smallImageText = null;

            var t = Task.Run(async () => {
                data = await provider.GetPlayerCountAsync(_sessionName);
            });

            t.Wait();

            if (data != null) {

                if (IsValidSessionDate(data.date)) {
                    _presence.details = $"{data.pc} players in session";
                } else {
                    _presence.details = "Session data out of date";
                }
            } else {
                _presence.details = $"Error communicating with session";
            }

            var dispatcher = Application.Current.Dispatcher;

            dispatcher.Invoke(() => {
                largeImageKey = TextBox_largeImageKey.Text;
                largeImageText = TextBox_largeImageText.Text;
                smallImageKey = TextBox_smallImageKey.Text;
                smallImageText = TextBox_smallImageText.Text;
            });

            _presence.state = "Time until next update:";
            _presence.startTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _presence.endTimestamp = (long)(DateTime.UtcNow.AddSeconds(30).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _presence.largeImageKey = string.IsNullOrEmpty(largeImageKey) ? "" : largeImageKey;
            _presence.largeImageText = string.IsNullOrEmpty(largeImageText) ? "" : largeImageText;
            _presence.smallImageKey = string.IsNullOrEmpty(smallImageKey) ? "" : smallImageKey;
            _presence.smallImageText = string.IsNullOrEmpty(smallImageText) ? "" : smallImageText;

            DiscordRpc.UpdatePresence(ref _presence);
        }


        private Boolean IsValidSessionDate(string timestamp) {
            DateTime givenTime;
            DateTime acceptableTime;


            givenTime = FromIso8601Date(timestamp);

            acceptableTime = DateTime.UtcNow.AddMinutes(5);

            // If the time we have been given is less than 5 minutes old, then it is acceptable.
            return (givenTime < acceptableTime);
        }


        public DateTime FromIso8601Date(string date) {
            DateTime dateTime;


            DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime);

            return dateTime;
        }
    }
}
