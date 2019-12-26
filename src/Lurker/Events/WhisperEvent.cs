//-----------------------------------------------------------------------
// <copyright file="WhisperEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public class WhisperEvent : PlayerEvent
    {
        #region Fields

        private static readonly string WhisperFromMarker = "@From";

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

            this.PlayerName = beforeMessageMarker.Substring(WhisperFromMarker.Length + 1);
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
            if (!IsWhisper(logLine))
            {
                return null;
            }

            return new WhisperEvent(logLine);
        }

        /// <summary>
        /// Determines whether the specified log line is whisper.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        protected static bool IsWhisper(string logLine)
        {
            var informations = ParseInformations(logLine);
            if (informations == null || !informations.StartsWith(WhisperFromMarker))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
