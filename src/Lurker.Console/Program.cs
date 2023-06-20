//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Console
{
    using System;
    using System.Collections.Generic;
    using Winook;

    class Program
    {
        private static readonly List<string> PossibleProcessNames = new List<string> { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam", "PathOfExile_x64_KG.exe", "PathOfExile_KG.exe" };

        static void Main(string[] args)
        {
        }

        private static void Hook_MessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            Console.WriteLine(e.KeyValue);
        }

        private static void Lurker_NewOffer(object sender, PoeLurker.Patreon.Events.TradeEvent e)
        {
            Console.WriteLine($"({e.Date})--[{e.GuildName}]{e.PlayerName} [item: {e.ItemName}] [price: {e.Price}] [position: {e.Location}] ");
        }

        private static void Watcher_Whispered(object sender, PoeLurker.Patreon.Events.WhisperEvent e)
        {
            Console.WriteLine($"[{e.GuildName}]{e.PlayerName}: {e.WhisperMessage}");
        }

        private static void Watcher_PlayerLeft(object sender, PoeLurker.Patreon.Events.PlayerLeftEvent e)
        {
            Console.WriteLine($"{e.PlayerName} left");
        }

        private static void Watcher_PlayerJoined(object sender, PoeLurker.Patreon.Events.PlayerJoinedEvent e)
        {
            Console.WriteLine($"{e.PlayerName} joined");
        }

        private static void Watcher_RemainingMonsters(object sender, PoeLurker.Patreon.Events.MonstersRemainEvent e)
        {
            Console.WriteLine($"Monster {e.MonsterCount}");
        }

        private static void Lurker_ChangedLocation(object sender, PoeLurker.Patreon.Events.LocationChangedEvent e)
        {
            Console.WriteLine(e.Location);
        }
    }
}
