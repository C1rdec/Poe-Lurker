//-----------------------------------------------------------------------
// <copyright file="WhisperEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    using System.Linq;

    public class WhisperEvent : PlayerEvent
    {
        #region Fields

        private static readonly string WhisperFromMarker = "@From";
        private static readonly string WhisperToMarker = "@To";

        #endregion

        #region Construrctors

        /// <summary>
        /// Initializes a new instance of the <see cref="WhisperEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        protected WhisperEvent(string logLine) 
            : base(logLine)
        {
            var index = this.Informations.IndexOf(MessageMarker);
            var beforeMessageMarker = this.Informations.Substring(0, index);

            this.PlayerName = string.Join(" ", beforeMessageMarker.Split(' ').Skip(1));
            var guildIndex = this.PlayerName.IndexOf(EndOfGuildNameMarker);

            if (guildIndex != -1)
            {
                this.GuildName = this.PlayerName.Substring(1, guildIndex - 1);
                this.PlayerName = this.PlayerName.Substring(guildIndex + 2);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the whisper message.
        /// </summary>
        public string WhisperMessage => this.Message;

        #endregion

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The whisperEvent</returns>
        public static WhisperEvent TryParse(string logLine)
        {
            if (!IsIncoming(logLine))
            {
                return null;
            }

            return new WhisperEvent(logLine);
        }

        /// <summary>
        /// Determines whether the specified log line is whisper.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        protected static bool IsIncoming(string logLine)
        {
            return StartWithMarker(WhisperFromMarker, logLine);
        }

        /// <summary>
        /// Determines whether the specified log line is outgoing.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        protected static bool IsOutgoing(string logLine)
        {
            return StartWithMarker(WhisperToMarker, logLine);
        }

        /// <summary>
        /// Tests the specified maker.
        /// </summary>
        /// <param name="maker">The maker.</param>
        /// <param name="logLine">The log line.</param>
        /// <returns>True if the line start with the marker.</returns>
        private static bool StartWithMarker(string maker, string logLine)
        {
            var informations = ParseInformations(logLine);
            if (informations == null || !informations.StartsWith(maker))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
