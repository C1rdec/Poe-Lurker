//-----------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    using Lurker.Items;
    using Lurker.Items.Extensions;
    using Lurker.Models;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TradeEvent : WhisperEvent
    {
        #region Fields

        private static readonly string[] GreetingMarkers = new string[] { "Hi, I would like to buy your", "Hi, I'd like to buy your", "wtb" };
        private static readonly string[] PriceMarkers = new string[] { "listed for", "for my" };
        private static readonly string LocationMarker = "(";
        private static readonly string LeagueMarker = " in ";
        private static readonly CurrencyTypeParser CurrencyTypeParser = new CurrencyTypeParser();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        public TradeEvent(string logLine)
            : base(logLine)
        {
            var priceMarker = PriceMarkers.FirstOrDefault(m => this.Message.Contains(m));
            var priceMarkerIndex = priceMarker == null ? -1 : this.Message.IndexOf(priceMarker);
            var leagueMarkerIndex = this.Message.IndexOf(LeagueMarker);

            // ItemName
            var itemIndex = priceMarkerIndex == -1 ? leagueMarkerIndex + 1 : priceMarkerIndex;
            var textBeforeMarker = this.Message.Substring(0, itemIndex);

            var greetingMarker = GreetingMarkers.FirstOrDefault(m => this.Message.Contains(m));
            this.ItemName = this.Message.Substring(greetingMarker.Length + 1, textBeforeMarker.Length - greetingMarker.Length -2);

            // Location
            var textAfterItemName = this.Message.Substring(itemIndex);
            var locationMarkerIndex = textAfterItemName.IndexOf(LocationMarker);
            if (locationMarkerIndex != -1)
            {
                this.Location = this.ParseLocation(textAfterItemName.Substring(locationMarkerIndex));
            }

            // Price
            if (priceMarkerIndex != -1)
            {
                var textAfterMarker = this.Message.Substring(priceMarkerIndex + priceMarker.Length + 1);
                var index = textAfterMarker.IndexOf(LeagueMarker);
                var priceValue = textAfterMarker.Substring(0, index);
                this.Price = this.ParsePrice(priceValue);
            }
            else
            {
                this.Price = new Price();
            }
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
        public Price Price { get; private set; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Location Location { get; private set; }

        #endregion

        #region Methods

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
            foreach (var greetingMarker in GreetingMarkers)
            {
                if (message.StartsWith(greetingMarker))
                {
                    return new TradeEvent(logLine); ;
                }
            }


            return null;
        }

        /// <summary>
        /// Parses the location.
        /// </summary>
        /// <param name="locationValue">The location value.</param>
        /// <returns>The item location</returns>
        public Location ParseLocation(string locationValue)
        {
            // Remove parentheses
            var value = locationValue.Substring(1, locationValue.Length - 2);

            // tab name
            var tabValue = value.GetLineBefore("\";");
            var index = tabValue.IndexOf("\"");
            var stashTabName = tabValue.Substring(index + 1);

            // Position
            var positionValue = value.GetLineAfter("position: ");
            var positions = positionValue.Split(", ");
            var left = positions[0].GetLineAfter("left ");
            var top = positions[1].GetLineAfter("top ");

            return new Location()
            {
                StashTabName = stashTabName,
                Left = Convert.ToInt32(left),
                Top = Convert.ToInt32(top),
            };
        }

        /// <summary>
        /// Parses the price.
        /// </summary>
        /// <param name="priceValue">The price value.</param>
        /// <returns>The price of the offer.</returns>
        public Price ParsePrice(string priceValue)
        {
            var values = priceValue.Split(' ');
            var currencyTypeValue = string.Join(" ", values.Skip(1));

            return new Price()
            {
                NumberOfCurrencies = double.Parse(values[0]),
                CurrencyType = CurrencyTypeParser.Parse(currencyTypeValue),
            };
        }

        /// <summary>
        /// Simplifies the name of the item.
        /// </summary>
        public string BuildSearchItemName()
        {
            var additionalInformationIndex = this.ItemName.IndexOf(" (");
            if (additionalInformationIndex != -1)
            {
                this.ItemName = this.ItemName.Substring(0, additionalInformationIndex);
            }

            var gemLevelIndex = this.ItemName.IndexOf("level ");
            if (gemLevelIndex != -1)
            {
                var gemDetails = this.ItemName.Split(' ');
                var quality = gemDetails[2];
                var gemName = string.Join(" ", gemDetails.Skip(3));

                return $"{gemName} {quality}";
            }

            return Regex.Replace(this.ItemName, @"[\d]", string.Empty).Trim();
        }

        #endregion
    }
}
