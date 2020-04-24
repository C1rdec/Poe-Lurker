//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Console
{
    using Lurker.Models;
    using Lurker.Services;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(new { Name = "Cedric", LastName = "Lampron"});
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var message = httpClient.PostAsync("https://us-central1-poe-lurker.cloudfunctions.net/sendTradeMessage", data).Result;

            var model = new Collaboration()
            {
                ExpireDate = DateTime.Now.AddDays(10)
            };

            var test = JsonConvert.SerializeObject(model);
            Console.WriteLine("Yeah");
            Console.Read();
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
