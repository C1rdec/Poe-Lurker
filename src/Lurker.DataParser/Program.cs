//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.DataParser
{
    using System;
    using System.IO;

    class Program
    {
        private static readonly string BasePath = @"..\..\..\..\assets\data\";
        private static readonly string ItemFilePath = Path.Combine(BasePath, "UniqueInfo.json");
        private static readonly string GemFilePath = Path.Combine(BasePath, "GemInfo.json");

        static void Main(string[] args)
        {
            File.WriteAllText(ItemFilePath, ItemParser.Parse());
            Console.WriteLine();
            File.WriteAllText(GemFilePath, GemParser.Parse());
        }
    }
}
