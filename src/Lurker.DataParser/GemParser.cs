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
    using Lurker.DataParser.Gems;
    using Lurker.Models;

    public class GemParser
    {
        #region Methods

        public static string Parse()
        {
            var gems = RePoeParser.Parse();
            //var gems = PathOfBuildingParser.Parse();

            var count = 1;
            var totalCount = gems.Count();
            foreach (var gem in gems)
            {
                Console.Write($"\rParsing Gems wiki information... ({count}/{totalCount})");
                gem.ParseWiki();
                count++;

                if (gem.Name.StartsWith("Vaal "))
                {
                    var normalGem = gems.FirstOrDefault(g => g.Name == gem.Name.Substring(5, gem.Name.Length - 5));
                    if (normalGem == null)
                    {
                        continue;
                    }

                    gem.Location = normalGem.Location;
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(gems, Newtonsoft.Json.Formatting.Indented);
        }

        private static IEnumerable<Gem> ParsePathOfBuilding()
        {
            return Enumerable.Empty<Gem>();
        }

        #endregion
    }
}
