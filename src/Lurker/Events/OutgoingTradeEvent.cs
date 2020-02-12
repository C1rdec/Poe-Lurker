//-----------------------------------------------------------------------
// <copyright file="OutgoingTradeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    /// <summary>
    /// Represents an Outgoing trade request.
    /// </summary>
    /// <seealso cref="Lurker.Events.TradeEvent" />
    public class OutgoingTradeEvent : TradeEvent
    {
        public OutgoingTradeEvent(string logLine) 
            : base(logLine)
        {
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The new Trade Event.</returns>
        public new static OutgoingTradeEvent TryParse(string logLine)
        {
            if (!IsOutgoing(logLine))
            {
                return null;
            }

            var message = ParseMessage(logLine);
            foreach (var greetingMarker in GreetingMarkers)
            {
                if (message.StartsWith(greetingMarker))
                {
                    return new OutgoingTradeEvent(logLine); ;
                }
            }

            return null;
        }
    }
}
