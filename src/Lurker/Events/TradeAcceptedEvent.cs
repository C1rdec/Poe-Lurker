//-----------------------------------------------------------------------
// <copyright file="TradeAcceptedEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public class TradeAcceptedEvent : PoeEvent
    {
        #region Fields

        private static readonly string TradeAcceptedMarker = "Trade accepted";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAcceptedEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        private TradeAcceptedEvent(string logLine) 
            : base(logLine)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The trade accepted event</returns>
        public static TradeAcceptedEvent TryParse(string logLine)
        {
            var informations = ParseInformations(logLine);
            if (string.IsNullOrEmpty(informations) || !informations.StartsWith($"{MessageMarker}{TradeAcceptedMarker}"))
            {
                return null;
            }

            return new TradeAcceptedEvent(logLine);
        }

        #endregion
    }
}
