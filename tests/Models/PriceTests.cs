using Lurker.Models;
using Lurker.Items.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.Models
{
    [TestClass]
    public class PriceTests
    {
        [TestMethod]
        public void FromLogLineReturnsObjectWhenPriceIsPresentTest()
        {
            string logLine = @"2020/01/17 21:48:39 160281265 ac9[INFO Client 4788] @From Привет_мир: " +
                @"Hi, I'd like to buy your 2 exalted for my 360 chaos in Metamorph.";

            var price = Price.FromLogLine(logLine);

            Assert.AreEqual(CurrencyType.Chaos, price.CurrencyType);
            Assert.AreEqual(360, price.NumberOfCurrencies);
        }

        [TestMethod]
        public void FromLogLineReturnsCorrectAmountOfCurrencyTest()
        {
            string logLine = @"2020/01/31 00:19:26 257478656 ac9 [INFO Client 16028] @From Ram_zDPS: " + 
                @"Hi, I would like to buy your Miracle Pelt Sharkskin Tunic listed for 1.1 exa in " + 
                @"Metamorph (stash tab ""~b / o 1.1 exa""; position: left 7, top 22) 1ex ?";

            var price = Price.FromLogLine(logLine);

            Assert.AreEqual(CurrencyType.Exalted, price.CurrencyType);
            Assert.AreEqual(1.1, price.NumberOfCurrencies);
        }

        [TestMethod]
        public void FromLogLineReturnsEmptyObjectWhenLogLineIsMalformed()
        {
            string logLine = @"2020/01/31 00:19:26 257478656 ac9 [INFO Client 16028] @From Ram_zDPS: " + 
                @"Hi, I would like to buy your Miracle Pelt Sharkskin Tunic listed for exa in Metamorph " +
                @"(stash tab ""~b/o 1.1 exa""; position: left 7, top 22) 1ex ?";

            var price = Price.FromLogLine(logLine);

            Assert.AreEqual(CurrencyType.Unknown, price.CurrencyType);
            Assert.AreEqual(0.0, price.NumberOfCurrencies);
        }
    }
}