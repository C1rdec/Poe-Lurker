//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Console
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Lurker.Helpers;
    using Lurker.Services;
    using Winook;

    class Program
    {
        private static readonly List<string> PossibleProcessNames = new List<string> { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam", "PathOfExile_x64_KG.exe", "PathOfExile_KG.exe" };

        static void Main(string[] args)
        {

            using (var ninjaService = new PathOfNinjaService())
            {
                var t = ninjaService.GetExaltRationAsync("Expedition").Result;
            }

            var processLurker = new PathOfExileProcessLurker();
            var processId = processLurker.WaitForProcess().Result;

            var hook = new KeyboardHook(processId);
            hook.AddHandler('F', Hook_MessageReceived);
            //hook.MessageReceived += Hook_MessageReceived;

            hook.InstallAsync().Wait();

            while (true)
            {
                Thread.Sleep(200);
            }
        }

        private static void Hook_MessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            Console.WriteLine(e.KeyValue);
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
