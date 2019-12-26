//-----------------------------------------------------------------------
// <copyright file="PlayerLeftEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public class PlayerLeftEvent : PlayerEvent
    {
        #region Fields

        private static readonly string LeftTheAreaMarker = "has left the area";

        #endregion

        #region Constructors

        private PlayerLeftEvent(string logLine) 
            : base(logLine)
        {
            var index = this.Message.IndexOf(LeftTheAreaMarker);
            this.SetPlayerName(index);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The event.</returns>
        public static PlayerLeftEvent TryParse(string logLine)
        {
            var informations = ParseInformations(logLine);
            if (string.IsNullOrEmpty(informations) || !informations.StartsWith(MessageMarker) || !informations.Contains(LeftTheAreaMarker))
            {
                return null;
            }

            return new PlayerLeftEvent(logLine);
        }

        #endregion
    }
}
