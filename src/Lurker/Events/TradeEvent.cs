//-----------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Models;
using System.Text.RegularExpressions;

namespace Lurker.Events
{
    public class TradeEvent : WhisperEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        public TradeEvent(string logLine)
            : base(logLine)
        { }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public Price Price { get; private set; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Gets or sets the note for the item.
        /// </summary>
        public string Note { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The new Trade Event.</returns>
        public new static TradeEvent TryParse(string logLine)
        {
            string pattern = @"(?:.*) @From (?:.*?): " + // Match the private message.
                @"(?:wtb|Hi, I would like to buy your|I'd like to buy your|Hi, I'd like to buy your) " + // Match the beginning of the trade message.
                @"(?:level [0-9]* [0-9]*% )*" + // Filter out unnecessary gem level information.
                @"(?<itemCount>[0-9 ]+)*(?<itemName>.*)" + // Create groups for item name and item count.
                @"(?: listed for | for my )(?:.*) in " + // Match the middle of the message to correctly group the item name.
                @"(?:.*)(?:[\.\)]+)" + // Match the League name and location info until the message ends with either a full stop or a closing bracket.
                @"(?<note>.*)"; // Group remaining text as additional note left by the user.

            Match match = Regex.Match(logLine, pattern);
            if (match.Success)
            {
                return new TradeEvent(logLine)
                {
                    ItemName = match.Groups["itemName"].Value.Trim(),
                    Location = Location.FromLogLine(logLine),
                    Price = Price.FromLogLine(logLine),
                    Note = match.Groups["note"].Value.Trim()
                };
            }

            return null;
        }

        #endregion Methods
    }
}