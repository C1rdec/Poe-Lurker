//-----------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    using Lurker.Extensions;
    using Lurker.Models;
    using Lurker.Parser;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TradeEvent : WhisperEvent
    {
        #region Fields

        protected static readonly string[] GreetingMarkers = new string[] { "Hi, I would like to buy your", "Hi, I'd like to buy your", "wtb" };
        private static readonly string[] PriceMarkers = new string[] { "listed for", "for my" };
        private static readonly string LocationMarker = "(";
        private static readonly string LocationMarkerEnd = ")";
        private static readonly string PositionMarker = "position: ";
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
            this.Location = this.ParseLocation(textAfterItemName);

            // Price
            if (priceMarkerIndex != -1)
            {
                var textAfterMarker = this.Message.Substring(priceMarkerIndex + priceMarker.Length + 1);
                var index = textAfterMarker.IndexOf(LeagueMarker);
                var priceValue = textAfterMarker;
                if (index != -1)
                {
                    priceValue = priceValue.Substring(0, index);
                }

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
        /// Determines whether [is trade message] [the specified message].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <c>true</c> if [is trade message] [the specified message]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTradeMessage(string message)
        {
            if (!message.StartsWith("@"))
            {
                return false;
            }

            var greetingMarker = GreetingMarkers.FirstOrDefault(m => message.Contains(m));
            if (string.IsNullOrEmpty(greetingMarker))
            {
                return false;
            }

            var priceMarker = PriceMarkers.FirstOrDefault(m => message.Contains(m));
            if (string.IsNullOrEmpty(priceMarker))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The new Trade Event.</returns>
        public new static TradeEvent TryParse(string logLine)
        {
            if (!IsIncoming(logLine))
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
            var locationMarkerIndex = locationValue.IndexOf(LocationMarker);
            var locationMarkerEndIndex = locationValue.IndexOf(LocationMarkerEnd);
            if (locationMarkerIndex == -1 || locationMarkerEndIndex == -1)
            {
                return new Location();
            }

            // tab name
            var tabValue = locationValue.GetLineBefore("\";");
            var index = tabValue.IndexOf("\"");
            var stashTabName = tabValue.Substring(index + 1);

            // Position
            var positionValue = locationValue.GetLineAfter("\";");
            var positionIndex = positionValue.IndexOf(PositionMarker);

            if (positionIndex != -1)
            {
                positionValue = positionValue.GetLineAfter("position: ");
            }

            var positions = positionValue.Split(", ");
            var leftValue = positions[0].GetLineAfter("left ");
            var topValue = positions[1].GetLineAfter("top ");

            if (string.IsNullOrEmpty(leftValue) || string.IsNullOrEmpty(topValue))
            {
                return new Location()
                {
                    StashTabName = stashTabName
                };
            }

            var closingMarkerIndex = topValue.IndexOf(")");
            topValue = topValue.Substring(0, closingMarkerIndex);

            return new Location()
            {
                StashTabName = stashTabName,
                Left = Convert.ToInt32(leftValue),
                Top = Convert.ToInt32(topValue),
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
                NumberOfCurrencies = double.Parse(values[0], CultureInfo.InvariantCulture),
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


        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var tradeEvent = obj as TradeEvent;
            if (tradeEvent == null)
            {
                return false;
            }

            if (tradeEvent.PlayerName != this.PlayerName)
            {
                return false;
            }

            if (tradeEvent.ItemName != this.ItemName)
            {
                return false;
            }

            if (!tradeEvent.Price.Equals(this.Price))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 1250757237;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ItemName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Price>.Default.GetHashCode(this.Price);
            hashCode = hashCode * -1521134295 + EqualityComparer<Location>.Default.GetHashCode(this.Location);
            return hashCode;
        }

        #endregion
    }
}
