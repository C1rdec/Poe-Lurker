//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Console
{
    using System;
    using System.Net.Http;
    using System.Text;
    using Lurker.Patreon.Events;
    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();

            while (true)
            {
                var tradeEvent = TradeEvent.TryParse("2020/04/13 00:38:33 611799171 acf [INFO Client 11220] @From <STONED> Resued_Delirium: Hi, I would like to buy your Hollow Fossil listed for 15 exalted in Delirium (stash tab \"qweq\"; position: left 3, top 7)");
                var json = JsonConvert.SerializeObject(new 
                { 
                    ItemName = tradeEvent.ItemName,
                    ItemClass = tradeEvent.ItemClass.ToString(),
                    WhisperMessage = tradeEvent.WhisperMessage,
                    PlayerName = tradeEvent.PlayerName,
                    Date = tradeEvent.Date,
                    Price = JsonConvert.SerializeObject(new { NumberOfCurrencies = tradeEvent.Price.NumberOfCurrencies.ToString(), CurrencyType = tradeEvent.Price.CurrencyType.ToString()})
                });

                var asd = JsonConvert.SerializeObject(tradeEvent);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var json = JsonConvert.SerializeObject(new { Test = "" });
                //var data = new StringContent(json , Encoding.UTF8, "application/json");
                var message = httpClient.PostAsync("https://us-central1-poe-lurker.cloudfunctions.net/sendTradeMessage", data).Result;
                Console.Read();
            }
        }

        private static void Lurker_NewOffer(object sender, Patreon.Events.TradeEvent e)
        {
            Console.WriteLine($"({e.Date})--[{e.GuildName}]{e.PlayerName} [item: {e.ItemName}] [price: {e.Price}] [position: {e.Location}] ");
        }

        private static void Watcher_Whispered(object sender, Patreon.Events.WhisperEvent e)
        {
            Console.WriteLine($"[{e.GuildName}]{e.PlayerName}: {e.WhisperMessage}");
        }

        private static void Watcher_PlayerLeft(object sender, Patreon.Events.PlayerLeftEvent e)
        {
            Console.WriteLine($"{e.PlayerName} left");
        }

        private static void Watcher_PlayerJoined(object sender, Patreon.Events.PlayerJoinedEvent e)
        {
            Console.WriteLine($"{e.PlayerName} joined");
        }

        private static void Watcher_RemainingMonsters(object sender, Patreon.Events.MonstersRemainEvent e)
        {
            Console.WriteLine($"Monster {e.MonsterCount}");
        }

        private static void Lurker_ChangedLocation(object sender, Patreon.Events.LocationChangedEvent e)
        {
            Console.WriteLine(e.Location);
        }
    }
}
