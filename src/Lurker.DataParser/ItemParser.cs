//-----------------------------------------------------------------------
// <copyright file="ItemParser.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.DataParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Patreon.Models;
    using Newtonsoft.Json.Linq;
    using NLua;

    public class ItemParser
    {
        #region Fields

        private static readonly string LevelMarker = "Requires Level";
        private static readonly string BaseUrl = "https://raw.githubusercontent.com/PathOfBuildingCommunity/PathOfBuilding/master/src/Data/Uniques";
        private static readonly string AmuletUrl = $"{BaseUrl}/amulet.lua";
        private static readonly string AxeUrl = $"{BaseUrl}/axe.lua";
        private static readonly string BeltUrl = $"{BaseUrl}/belt.lua";
        private static readonly string BodyUrl = $"{BaseUrl}/body.lua";
        private static readonly string BootsUrl = $"{BaseUrl}/boots.lua";
        private static readonly string BowUrl = $"{BaseUrl}/bow.lua";
        private static readonly string ClawUrl = $"{BaseUrl}/claw.lua";
        private static readonly string DaggerUrl = $"{BaseUrl}/dagger.lua";
        private static readonly string FlaskUrl = $"{BaseUrl}/flask.lua";
        private static readonly string GlovesUrl = $"{BaseUrl}/gloves.lua";
        private static readonly string HelmetUrl = $"{BaseUrl}/helmet.lua";
        private static readonly string JewelUrl = $"{BaseUrl}/jewel.lua";
        private static readonly string MaceUrl = $"{BaseUrl}/mace.lua";
        private static readonly string QuiverUrl = $"{BaseUrl}/quiver.lua";
        private static readonly string RingUrl = $"{BaseUrl}/ring.lua";
        private static readonly string ShieldUrl = $"{BaseUrl}/shield.lua";
        private static readonly string StaffUrl = $"{BaseUrl}/staff.lua";
        private static readonly string SwordUrl = $"{BaseUrl}/sword.lua";
        private static readonly string WandUrl = $"{BaseUrl}/wand.lua";

        private static readonly (string Url, ItemClass ItemClass)[] ItemUrls = new (string, ItemClass)[]
        { 
            (AmuletUrl, ItemClass.Amulet),
            (AxeUrl, ItemClass.Axe),
            (BeltUrl, ItemClass.Belt),
            (BodyUrl, ItemClass.BodyArmour),
            (BootsUrl, ItemClass.Boots),
            (BowUrl, ItemClass.Bow),
            (ClawUrl, ItemClass.Claw),
            (DaggerUrl, ItemClass.Dagger),
            (FlaskUrl, ItemClass.Flask),
            (GlovesUrl, ItemClass.Gloves),
            (HelmetUrl, ItemClass.Helmet),
            (JewelUrl, ItemClass.Jewel),
            (MaceUrl, ItemClass.Mace),
            (QuiverUrl, ItemClass.Quiver),
            (RingUrl, ItemClass.Ring),
            (ShieldUrl, ItemClass.Shield),
            (StaffUrl, ItemClass.Staff),
            (SwordUrl, ItemClass.Sword),
            (WandUrl, ItemClass.Wand),
        };

        #endregion

        #region Methods

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns></returns>
        public static string Parse()
        {
            var client = new HttpClient();
            var tempFolder = GetTemporaryDirectory();

            var uniqueItems = new List<UniqueItem>();
            foreach (var uniqueItem in ItemUrls)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uniqueItem.Url);
                var response = client.SendAsync(request).Result;

                var itemLuaInfo = response.Content.ReadAsStringAsync().Result;
                var fileName = Path.Combine(tempFolder, uniqueItem.ItemClass.ToString());
                File.WriteAllText(fileName, itemLuaInfo);
                using (var lua = new Lua())
                {
                    var luaObjects = lua.DoFile(fileName);
                    var firstLuaObject = luaObjects.FirstOrDefault();
                    if (firstLuaObject == null)
                    {
                        continue;
                    }

                    if (firstLuaObject is LuaTable table)
                    {
                        foreach (var value in table.Values)
                        {
                            Console.Write($"\rParsing Unique Items information... ({uniqueItems.Count()})");

                            if (value is string stringValue)
                            {
                                var lines = stringValue.Split(System.Environment.NewLine.ToCharArray());
                                var levelLine = lines.FirstOrDefault(l => l.StartsWith(LevelMarker));
                                
                                var nameLine = lines.FirstOrDefault();
                                if (!string.IsNullOrEmpty(nameLine))
                                {
                                    var item = new UniqueItem()
                                    {
                                        Name = nameLine,
                                        ItemClass = uniqueItem.ItemClass,
                                        WikiUrl = WikiHelper.CreateItemUri(nameLine),
                                        Level = GetLevel(levelLine),
                                    };

                                    try
                                    {
                                        item.ImageUrl = WikiHelper.GetItemImageUrl(nameLine);
                                    }
                                    catch(InvalidOperationException)
                                    {
                                    }

                                    uniqueItems.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(uniqueItems, Newtonsoft.Json.Formatting.Indented);
        }

        private static int GetLevel(string levelLine)
        {
            if (string.IsNullOrEmpty(levelLine))
            {
                return 1;
            }

            var levelValue = levelLine.Substring(LevelMarker.Length).Trim(' ', ':');
            levelValue = levelValue.Split(',').FirstOrDefault();
            return Convert.ToInt32(levelValue);
        }

        private static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        #endregion
    }
}
