namespace SessionPresenceUpdater.Providers
{
    /// <summary>
    /// Defines a session.
    /// </summary>
    public class SessionDetail
    {

        /// <summary>
        /// The UTC date/time of the session details.
        /// </summary>
        public string date;


        /// <summary>
        /// The number of players in the session.
        /// </summary>
        public int pc;


        /// <summary>
        /// The <see cref="SessionPlayer"/>'s in the session.
        /// </summary>
        public SessionPlayer[] players;


        /// <summary>
        /// The name of the session.
        /// </summary>
        public string session;
    }
}
