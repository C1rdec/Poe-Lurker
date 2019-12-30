﻿//-----------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    using Lurker.Models;
    using System;
    using System.Linq;

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
            var leagueMarkerIndex = this.Message.IndexOf(LeagueMarker);

            // ItemName
            var itemIndex = priceMarkerIndex == -1 ? leagueMarkerIndex : priceMarkerIndex;
            var textBeforeMarker = this.Message.Substring(0, itemIndex);
            this.ItemName = this.Message.Substring(GreetingMarker.Length + 1, textBeforeMarker.Length - GreetingMarker.Length -1);

            // Location
            var locationMarkerIndex = this.Message.IndexOf(LocationMarker);
            this.Position = this.Message.Substring(locationMarkerIndex);

            // Price
            if (priceMarkerIndex != -1)
            {
                var textAfterMarker = this.Message.Substring(priceMarkerIndex + PriceMarker.Length + 1);
                var index = textAfterMarker.IndexOf(LeagueMarker);
                var priceValue = textAfterMarker.Substring(0, index);
                this.Price = ParsePrice(priceValue);
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
        public string Position { get; private set; }

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
            if (!message.StartsWith(GreetingMarker))
            {
                return null;
            }

            return new TradeEvent(logLine);
        }

        /// <summary>
        /// Parses the price.
        /// </summary>
        /// <param name="priceValue">The price value.</param>
        /// <returns>The price of the offer.</returns>
        public static Price ParsePrice(string priceValue)
        {
            var values = priceValue.Split(' ');
            var currencyTypeValue = string.Join(" ", values.Skip(1));

            CurrencyType type;
            if(!Enum.TryParse(currencyTypeValue, true, out type))
            {
                type = CurrencyType.Unknown;
            }

            return new Price()
            {
                NumberOfCurrencies = int.Parse(values[0]),
                CurrencyType = type
            };
        }

        #endregion

    }
}
