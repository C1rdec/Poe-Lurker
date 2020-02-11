using Lurker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.Models
{
    [TestClass]
    public class LocationTests
    {
        [TestMethod]
        public void FromLogLineReturnsCorrectObjectWhenLocationIsPresentTest()
        {
            string logLine = @"2020/01/31 00:19:26 257478656 ac9 [INFO Client 16028] @From абвв: " +
                @"Hi, I would like to buy your Mao Kun Shore Map (T4) listed for 5 chaos in Metamorph " +
                @"(stash tab ""~price 5 chaos""; position: left 2, top 12)";

            var loc = Location.FromLogLine(logLine);

            Assert.AreEqual("~price 5 chaos", loc.StashTabName);
            Assert.AreEqual(2, loc.Left);
            Assert.AreEqual(12, loc.Top);
        }

        [TestMethod]
        public void FromLogLineReturnsCorrectObjectWhenLocationIsMissingPositionTest()
        {
            string logLine = @"2020/02/09 23:29:54 361878531 ac9[INFO Client 19460] @From test_name_1: " +
                @"wtb Fire and Ice listed for 5 Chaos Orb in Metamorph(stash ""~price 5 chaos""; left 1, top 12)";

            var loc = Location.FromLogLine(logLine);

            Assert.AreEqual("~price 5 chaos", loc.StashTabName);
            Assert.AreEqual(1, loc.Left);
            Assert.AreEqual(12, loc.Top);
        }

        [TestMethod]
        public void FromLogLineReturnsEmptyObjectWhenLocationIsMissingTest()
        {
            string logLine = @"2020/01/17 21:48:39 160281265 ac9[INFO Client 4788] @From 안녕 세상: " +
                @"I'd like to buy your 1 Hall of Grandmasters Promenade Map (T16) for my 10 Silver Coin in Metamorph.";

            var loc = Location.FromLogLine(logLine);

            Assert.IsNull(loc.StashTabName);
            Assert.AreEqual(0, loc.Left);
            Assert.AreEqual(0, loc.Top);
        }

        [TestMethod]
        public void FromLogLineReturnsEmptyObjectWhenLocationIsMalformedTest()
        {
            string logLine = @"2020/01/17 21:48:39 160281265 ac9[INFO Client 4788] @From 안녕 세상: " +
                @"wtb Fire and Ice listed for 5 Chaos Orb in Metamorph(stash ""~price 5 chaos""; left a, top b)";

            var loc = Location.FromLogLine(logLine);

            Assert.IsNull(loc.StashTabName);
            Assert.AreEqual(0, loc.Left);
            Assert.AreEqual(0, loc.Top);
        }
    }
}