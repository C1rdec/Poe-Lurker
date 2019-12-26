//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Console
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"C:\Program Files (x86)\Steam\steamapps\common\Path of Exile\logs\Client.txt";
            using (var watcher = new ClientWatcher(filePath))
            {
                watcher.LocationChanged += Lurker_ChangedLocation;
                watcher.RemainingMonsters += Watcher_RemainingMonsters;
                watcher.PlayerJoined += Watcher_PlayerJoined;
                watcher.PlayerLeft += Watcher_PlayerLeft;
                Console.Read();
            }
        }

        private static void Watcher_PlayerLeft(object sender, Events.PlayerLeftEvent e)
        {
            Console.WriteLine($"{e.PlayerName} left");
        }

        private static void Watcher_PlayerJoined(object sender, Events.PlayerJoinedEvent e)
        {
            Console.WriteLine($"{e.PlayerName} joined");
        }

        private static void Watcher_RemainingMonsters(object sender, Events.MonstersRemainEvent e)
        {
            Console.WriteLine($"Monster {e.MonsterCount}");
        }

        private static void Lurker_ChangedLocation(object sender, Events.LocationChangedEvent e)
        {
            Console.WriteLine(e.Location);
        }
    }
}
