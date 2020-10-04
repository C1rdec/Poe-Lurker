//-----------------------------------------------------------------------
// <copyright file="GemParser.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.DataParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Newtonsoft.Json.Linq;

    public class GemParser
    {
        #region Methods

        public static string Parse()
        {
            var fileUrl = "https://raw.githubusercontent.com/brather1ng/RePoE/master/RePoE/data/gems.json";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
            var response = client.SendAsync(request).Result;

            var gemInfo = response.Content.ReadAsStringAsync().Result;

            var jobject = JObject.Parse(gemInfo);

            var gems = new List<Models.Gem>();
            foreach (var element in jobject.Children())
            {
                Console.Write($"\rParsing Gems information... ({gems.Count})");

                var children = element.Children();

                var requiredLevel = children["per_level"]["1"]["required_level"].FirstOrDefault();
                if (requiredLevel == null)
                {
                    continue;
                }

                var isSupport = (bool)children["is_support"].FirstOrDefault();
                var id = ((Newtonsoft.Json.Linq.JProperty)element).Name;

                var baseItem = children["base_item"].FirstOrDefault();
                if (baseItem is JValue)
                {
                    continue;
                }

                var displayName = (string)children["base_item"]["display_name"].FirstOrDefault();
                var gem = new Models.Gem()
                {
                    Id = id,
                    Name = displayName,
                    Level = (int)requiredLevel,
                    Support = isSupport,
                };

                gem.ParseWiki();
                gems.Add(gem);
            }

            // Update Location for Vaal Gems
            foreach (var gem in gems.Where(g => g.Name.StartsWith("Vaal ")))
            {
                var normalGem = gems.FirstOrDefault(g => g.Name == gem.Name.Substring(5, gem.Name.Length - 5));
                if (normalGem == null)
                {
                    continue;
                }

                gem.Location = normalGem.Location;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(gems);
        }

        #endregion
    }
}
