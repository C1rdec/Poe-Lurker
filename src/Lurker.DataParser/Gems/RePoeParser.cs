//-----------------------------------------------------------------------
// <copyright file="RePoeParser.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.DataParser.Gems
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Lurker.Models;
    using Newtonsoft.Json.Linq;

    public static class RePoeParser
    {
        public static IEnumerable<Gem> Parse()
        {
            var fileUrl = "https://raw.githubusercontent.com/brather1ng/RePoE/master/RePoE/data/gems.json";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
            var response = client.SendAsync(request).Result;

            var gemInfo = response.Content.ReadAsStringAsync().Result;

            var jobject = JObject.Parse(gemInfo);

            var gems = new List<Gem>();
            foreach (var element in jobject.Children())
            {
                var children = element.Children();

                var requiredLevel = children["per_level"]["1"]["required_level"].FirstOrDefault();
                if (requiredLevel == null)
                {
                    continue;
                }

                var isSupport = (bool)children["is_support"].FirstOrDefault();
                var id = ((JProperty)element).Name;

                var baseItem = children["base_item"].FirstOrDefault();
                if (baseItem is JValue)
                {
                    continue;
                }

                var displayName = (string)children["base_item"]["display_name"].FirstOrDefault();
                var gem = new Gem()
                {
                    Id = id,
                    Name = displayName,
                    Level = (int)requiredLevel,
                    Support = isSupport,
                };

                gems.Add(gem);
            }

            return gems;
        }
    }
}
