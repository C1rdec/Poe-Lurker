using Lurker.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace tests.Events
{
    [TestClass]
    public class TradeEventTests
    {
        [TestMethod]
        public void TryParseReturnsNullWhenLogLineIsNotWhisperTest()
        {
            List<string> log_lines = new List<string>
            {
                @"2020/02/07 09:42:45 ***** LOG FILE OPENING *****",
                @"2020/02/07 09:42:45 600605633 1c7 [INFO Client 8] Send patching protocol version 5",
                @"2020/02/07 09:42:45 600605677 23d [INFO Client 8] Web root: http://patch.poecdn.com/patch/3.9.2.6/",
                @"2020/02/07 09:42:45 600605677 23e [INFO Client 8] Backup Web root: http://patchcdn.pathofexile.com/3.9.2.6/",
                @"2020/02/07 09:42:45 600605677 249 [INFO Client 8] Requesting root contents 1",
                @"2020/02/07 09:42:45 600605695 273 [INFO Client 8] Got file list for "" 0",
                @"2020/02/07 09:44:22 600702850 20c [DEBUG Client 67] CreateSwapChain: SetFullScreenState 0"
            };

            foreach (string log_line in log_lines)
            {
                Assert.IsNull(TradeEvent.TryParse(log_line));
            }
        }

        [TestMethod]
        public void TryParseReturnsCorrectObjectWhenLogLineIsParseableTest()
        {
            List<string> log_lines = new List<string>
            {
                @"2020/02/09 23:29:54 361878531 ac9[INFO Client 19460] @From test_name_1: wtb Fire and Ice listed for 5 Chaos Orb in Metamorph(stash ""~price 5 chaos""; left 1, top 12)",
                @"2020/02/09 23:04:49 360373843 ac9[INFO Client 19460] @From < Some Guild > Test Name: Hi, I would like to buy your Morbid Portent Ghastly Eye Jewel listed for 15 chaos in Metamorph(stash tab ""~b/o 15 chaos""; position: left 6, top 23)",
                @"2020/01/17 21:48:39 160281265 ac9[INFO Client 4788] @From 안녕 세상: I'd like to buy your 1 Hall of Grandmasters Promenade Map (T16) for my 10 Silver Coin in Metamorph.",
                @"2020/01/17 21:48:39 160281265 ac9[INFO Client 4788] @From Привет_мир: Hi, I'd like to buy your 2 exalted for my 360 chaos in Metamorph. how does 3c sound?",
                @"2020/01/31 00:19:26 257478656 ac9[INFO Client 16028] @From абвв: Hi, I would like to buy your Mao Kun Shore Map(T4) listed for 5 chaos in Metamorph(stash tab ""~price 5 chaos""; position: left 2, top 12) how about 3c ?",
                @"2020/01/31 00:19:26 257478656 ac9 [INFO Client 16028] @From Ram_zDPS: Hi, I would like to buy your Miracle Pelt Sharkskin Tunic listed for 1.1 exa in Metamorph (stash tab ""~b/o 1.1 exa""; position: left 7, top 22)1ex ?",
                @"2020/01/31 00:19:26 257478656 ac9[INFO Client 16028] @From Luke_Flystalker: Hi, I would like to buy your level 20 23% Spirit Offering listed for 1.1 exa in Metamorph(stash tab ""tekoop""; position: left 3, top 11)"
            };

            var tevt = TradeEvent.TryParse(log_lines[0]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "Fire and Ice");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Chaos);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 5);
            Assert.AreEqual(tevt.Location.StashTabName, "~price 5 chaos");
            Assert.AreEqual(tevt.Location.Left, 1);
            Assert.AreEqual(tevt.Location.Top, 12);
            Assert.AreEqual(tevt.Note, "");
            tevt = TradeEvent.TryParse(log_lines[1]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "Morbid Portent Ghastly Eye Jewel");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Chaos);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 15);
            Assert.AreEqual(tevt.Location.StashTabName, "~b/o 15 chaos");
            Assert.AreEqual(tevt.Location.Left, 6);
            Assert.AreEqual(tevt.Location.Top, 23);
            Assert.AreEqual(tevt.Note, "");
            tevt = TradeEvent.TryParse(log_lines[2]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "Hall of Grandmasters Promenade Map (T16)");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Unknown);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 10);
            Assert.IsNull(tevt.Location.StashTabName);
            Assert.AreEqual(tevt.Location.Left, 0);
            Assert.AreEqual(tevt.Location.Top, 0);
            Assert.AreEqual(tevt.Note, "");
            tevt = TradeEvent.TryParse(log_lines[3]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "exalted");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Chaos);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 360);
            Assert.IsNull(tevt.Location.StashTabName);
            Assert.AreEqual(tevt.Location.Left, 0);
            Assert.AreEqual(tevt.Location.Top, 0);
            Assert.AreEqual(tevt.Note, "how does 3c sound?");
            tevt = TradeEvent.TryParse(log_lines[4]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "Mao Kun Shore Map(T4)");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Chaos);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 5);
            Assert.AreEqual(tevt.Location.StashTabName, "~price 5 chaos");
            Assert.AreEqual(tevt.Location.Left, 2);
            Assert.AreEqual(tevt.Location.Top, 12);
            Assert.AreEqual(tevt.Note, "how about 3c ?");
            tevt = TradeEvent.TryParse(log_lines[5]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "Miracle Pelt Sharkskin Tunic");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Exalted);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 1.1);
            Assert.AreEqual(tevt.Location.StashTabName, "~b/o 1.1 exa");
            Assert.AreEqual(tevt.Location.Left, 7);
            Assert.AreEqual(tevt.Location.Top, 22);
            Assert.AreEqual(tevt.Note, "1ex ?");
            tevt = TradeEvent.TryParse(log_lines[6]);
            Assert.IsNotNull(tevt);
            Assert.AreEqual(tevt.ItemName, "Spirit Offering");
            Assert.AreEqual(tevt.Price.CurrencyType, Lurker.Items.Models.CurrencyType.Exalted);
            Assert.AreEqual(tevt.Price.NumberOfCurrencies, 1.1);
            Assert.AreEqual(tevt.Location.StashTabName, "tekoop");
            Assert.AreEqual(tevt.Location.Left, 3);
            Assert.AreEqual(tevt.Location.Top, 11);
            Assert.AreEqual(tevt.Note, "");
        }
    }
}