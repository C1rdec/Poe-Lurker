//-----------------------------------------------------------------------
// <copyright file="TradeEventTest.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Test.Events
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using PoeLurker.Patreon.Events;
    using PoeLurker.Patreon.Models;

    [TestClass]
    public class TradeEventTest
    {
        #region Fields

        private MockRepository _mockRepository;

        private static string PlayerName = "PlayerName";

        private static string Date = "2020/03/12 20:21:05 ";

        private static string Information = "130518546 ac9 [INFO Client 15324]";

        private static string League = "in Standard";

        private static string ItemName = "Squandered Spike";

        private static string Location = "(stash tab \"Items\"; position: left 10, top 2)";

        private static string Greeting = "Hi, I would like to buy your";

        private static string Price = "listed for 1 chaos";

        #endregion

        #region Properties

        private string BaseLogLine => $"{Date} {Information} @From {PlayerName}: {Greeting}";

        private Price ExpectedPrice => new Price() { NumberOfCurrencies = 1, CurrencyType = CurrencyType.Chaos };

        private Location ExpectedLocation => new Location() { StashTabName = "Items", Left = 10, Top = 2 };

        #endregion

        [TestInitialize]
        public void Initialize()
        {
            this._mockRepository = new MockRepository(MockBehavior.Strict);
        }


        [TestMethod]
        public void TryParse()
        {
            var tradeEvent = TradeEvent.TryParse($"{BaseLogLine} {ItemName} {Price} {League} {Location}");

            Assert.AreEqual(PlayerName, tradeEvent.PlayerName);
            Assert.AreEqual(ItemName, tradeEvent.ItemName);
            this.AssertPrice(this.ExpectedPrice, tradeEvent.Price);
            this.AssertLocation(this.ExpectedLocation, tradeEvent.Location);
        }

        [TestMethod]
        public void TryParse_NoGreeting()
        {
            var tradeEvent = TradeEvent.TryParse($"{Date} {Information} @From {PlayerName}:{ItemName} {Price} {League} {Location}");
            Assert.IsNull(tradeEvent);
        }

        [TestMethod]
        public void TryParse_TextAfter()
        {
            var tradeEvent = TradeEvent.TryParse($"{BaseLogLine} {ItemName} {Price} {League} {Location} 10c?");

            Assert.AreEqual(PlayerName, tradeEvent.PlayerName);
            Assert.AreEqual(ItemName, tradeEvent.ItemName);
            this.AssertPrice(this.ExpectedPrice, tradeEvent.Price);
            this.AssertLocation(this.ExpectedLocation, tradeEvent.Location);
        }

        [TestMethod]
        public void TryParse_NoPrice()
        {
            var tradeEvent = TradeEvent.TryParse($"{BaseLogLine} {ItemName} {League} {Location}");
            var expectedPrice = new Price();
            var actualPrice = tradeEvent.Price;

            Assert.AreEqual(PlayerName, tradeEvent.PlayerName);
            Assert.AreEqual(ItemName, tradeEvent.ItemName);
            this.AssertPrice(expectedPrice, actualPrice);
            this.AssertLocation(this.ExpectedLocation, tradeEvent.Location);
        }

        [TestMethod]
        public void TryParse_NoLocation()
        {
            var tradeEvent = TradeEvent.TryParse($"{BaseLogLine} {ItemName} {Price} {League}");
            var actualPrice = tradeEvent.Price;

            Assert.AreEqual(PlayerName, tradeEvent.PlayerName);
            Assert.AreEqual(ItemName, tradeEvent.ItemName);
            this.AssertPrice(this.ExpectedPrice, actualPrice);
            this.AssertLocation(new Location(), tradeEvent.Location);
        }

        [TestMethod]
        public void TryParse_NoStashTabName()
        {
            var line = $"{BaseLogLine} {ItemName} {Price} {League} (stash tab ; position: left 10, top 2)";
            var tradeEvent = TradeEvent.TryParse(line);

            Assert.AreEqual(PlayerName, tradeEvent.PlayerName);
            Assert.AreEqual(ItemName, tradeEvent.ItemName);
            this.AssertPrice(this.ExpectedPrice, tradeEvent.Price);
            this.AssertLocation(new Location() { Top = 2, Left = 10, StashTabName = string.Empty }, tradeEvent.Location);
        }

        private void AssertPrice(Price expectedPrice, Price actualPrice)
        {
            Assert.AreEqual(expectedPrice.NumberOfCurrencies, actualPrice.NumberOfCurrencies);
            Assert.AreEqual(expectedPrice.CurrencyType, actualPrice.CurrencyType);
        }

        private void AssertLocation(Location expectedLocation, Location actualLocation)
        {
            Assert.AreEqual(expectedLocation.StashTabName, actualLocation.StashTabName);
            Assert.AreEqual(expectedLocation.Top, actualLocation.Top);
            Assert.AreEqual(expectedLocation.Left, actualLocation.Left);
        }

        /// <summary>
        /// Builds the expected trade event.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private TradeEvent BuildExpectedTradeEvent(string line)
        {
            return TradeEvent.TryParse(line);
        }
    }
}
