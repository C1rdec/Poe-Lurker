//-----------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public class TradeEvent : WhisperEvent
    {
        #region Fields

        private static readonly string GreetingMarker = "Hi, I would like to buy your";
        private static readonly string PriceMarker = "listed for";
        private static readonly string LocationMarker = "(";
        private static readonly string LeagueMarker = " in ";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        public TradeEvent(string logLine)
            : base(logLine)
        {
            var priceMarkerIndex = this.Message.IndexOf(PriceMarker);

            // ItemName
            var textBeforeMarker = this.Message.Substring(0, priceMarkerIndex);
            this.ItemName = this.Message.Substring(GreetingMarker.Length + 1, textBeforeMarker.Length - GreetingMarker.Length -2);

            // Location
            var locationMarkerIndex = this.Message.IndexOf(LocationMarker);
            this.Position = this.Message.Substring(locationMarkerIndex);

            // Price
            var textAfterMarker = this.Message.Substring(priceMarkerIndex + PriceMarker.Length);
            var leagueMarkerIndex = textAfterMarker.IndexOf(LeagueMarker);
            this.Price = textAfterMarker.Substring(0, leagueMarkerIndex);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public string Price { get; private set; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public string Position { get; private set; }

        #endregion

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The new Trade Event.</returns>
        public new static TradeEvent TryParse(string logLine)
        {
            if (!IsWhisper(logLine))
            {
                return null;
            }

            var message = ParseMessage(logLine);
            if (!message.StartsWith(GreetingMarker))
            {
                return null;
            }

            return new TradeEvent(logLine);
        }
    }
}
