//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Uninstall
{
    using System;
    using System.Diagnostics;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderName = "PoeLurker";
            var fileName = "Update.exe";

            var existingProcess = Process.GetProcessesByName("PoeLurker");
            if (existingProcess.Length > 0)
            {
                Console.WriteLine("Poe Lurker is running.");
                Console.ReadLine();
                return;
            }

            Process.Start(Path.Combine(localAppData, folderName, fileName), "--uninstall");
            Console.WriteLine("Uninstall successfully");
            Console.ReadLine();
        }
    }
}
