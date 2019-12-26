//-----------------------------------------------------------------------
// <copyright file="PlayerJoinedEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public class PlayerJoinedEvent: PlayerEvent
    {
        #region Fields

        private static readonly string JoinTheAreaMarker = "has joined the area";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerJoinedEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        private PlayerJoinedEvent(string logLine)
            : base(logLine)
        {
            var index = this.Message.IndexOf(JoinTheAreaMarker);
            this.SetPlayerName(index);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The event.</returns>
        public static PlayerJoinedEvent TryParse(string logLine)
        {
            var informations = ParseInformations(logLine);
            if (string.IsNullOrEmpty(informations) || !informations.StartsWith(MessageMarker) || !informations.Contains(JoinTheAreaMarker))
            {
                return null;
            }

            return new PlayerJoinedEvent(logLine);
        }

        #endregion
    }
}
