//-----------------------------------------------------------------------
// <copyright file="PathOfBuildingParser.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.DataParser.Gems
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Lurker.Models;
    using NLua;

    public static class PathOfBuildingParser
    {
        private static readonly string GemsUrl = "https://raw.githubusercontent.com/PathOfBuildingCommunity/PathOfBuilding/master/src/Data/Gems.lua";


        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Gem> Parse()
        {
            var client = new HttpClient();
            var tempFolder = GetTemporaryDirectory();

            var gems = new List<Gem>();
            var request = new HttpRequestMessage(HttpMethod.Get, GemsUrl);
            var response = client.SendAsync(request).Result;

            var itemLuaInfo = response.Content.ReadAsStringAsync().Result;
            var fileName = Path.Combine(tempFolder, "test.lua");
            File.WriteAllText(fileName, itemLuaInfo);

            using (var lua = new Lua())
            {
                var luaObjects = lua.DoFile(fileName);
                var firstLuaObject = luaObjects.FirstOrDefault();
            }

            return gems;
        }

        private static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
