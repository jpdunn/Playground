using System.Windows;

namespace SessionPresenceUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // 32bit Discord RPC DLL. Will come packed with this project.
        public const string DLL = "discord-rpc-w32";

        public App() : base()
        {
            if (!System.IO.File.Exists(DLL + ".dll"))
            {
                MessageBox.Show(
                    "Missing " + DLL + ".dll\n\n" +
                    "Please get it and put it alongside SessionPresenceUpdate.exe."
                );

                this.Shutdown();
            }
        }
    }
}
