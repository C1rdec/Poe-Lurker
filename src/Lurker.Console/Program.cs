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

    class Program
    {
        static void Main(string[] args)
        {
            var model = new Collaboration()
            {
                ExpireDate = DateTime.Now.AddDays(10)
            };

            var test = JsonConvert.SerializeObject(model);

            //var lurker = new ClipboardLurker();
            using (var lurker = new ClientLurker())
            {
                lurker.WaitForPoe().Wait();
                Console.WriteLine("Poe Found");
                lurker.LocationChanged += Lurker_ChangedLocation;
                lurker.RemainingMonsters += Watcher_RemainingMonsters;
                lurker.PlayerJoined += Watcher_PlayerJoined;
                lurker.PlayerLeft += Watcher_PlayerLeft;
                lurker.Whispered += Watcher_Whispered;
                lurker.IncomingOffer += Lurker_NewOffer;
                Console.Read();
            }

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
