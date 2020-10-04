//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.DataParser
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            System.IO.File.WriteAllText("c:/temp/itemInfo.json", ItemParser.Parse());
            Console.WriteLine();
            System.IO.File.WriteAllText("c:/temp/gemsInfo.json", GemParser.Parse());
        }
    }
}
