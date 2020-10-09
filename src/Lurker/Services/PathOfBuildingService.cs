//-----------------------------------------------------------------------
// <copyright file="PathOfBuildingService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Lurker.Extensions;
    using Lurker.Models;

    /// <summary>
    /// Represents the service for Path ofBuilding.
    /// </summary>
    public class PathOfBuildingService : HttpServiceBase
    {
        #region Fields

        private List<Gem> _knownGems;
        private List<UniqueItem> _knownUniques;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// /// <returns>
        /// The task awaiter.
        /// </returns>
        public async Task InitializeAsync()
        {
            var gemInformation = await this.GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Data/GemInfo.json?{Guid.NewGuid()}");
            var uniqueInformation = await this.GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Data/UniqueInfo.json?{Guid.NewGuid()}");
            this._knownGems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Gem>>(gemInformation);
            this._knownUniques = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UniqueItem>>(uniqueInformation);
        }

        /// <summary>
        /// Decodes the specified build.
        /// </summary>
        /// <param name="buildValue">The build value.</param>
        /// <returns>
        /// The xml structure.
        /// </returns>
        public Build Decode(string buildValue)
        {
            if (this._knownGems == null)
            {
                throw new InvalidOperationException("Must be initialized");
            }

            var build = new Build()
            {
                Value = buildValue,
            };

            var document = XDocument.Parse(GetXml(buildValue));
            var skillsElement = document.Root.Element("Skills");
            if (skillsElement != null)
            {
                foreach (var element in skillsElement.Elements())
                {
                    var skill = Skill.FromXml(element, this._knownGems);
                    if (skill.Gems.Any())
                    {
                        build.AddSkill(skill);
                    }
                }
            }

            var treeElement = document.Root.Element("Tree");
            if (treeElement != null)
            {
                var urlElement = treeElement.Descendants("URL").FirstOrDefault();
                build.SkillTreeUrl = urlElement.Value.Trim().Replace("passive-skill-tree", "fullscreen-passive-skill-tree");
            }

            var itemsElement = document.Root.Element("Items");
            foreach (var element in itemsElement.Elements())
            {
                var value = element.Value.GetLineAfter("Rarity: ");
                if (value != null)
                {
                    var lines = value.Split('\n');
                    var rarity = lines.FirstOrDefault();
                    if (rarity == "UNIQUE" && lines.Length > 2)
                    {
                        var name = lines[1];
                        var uniqueItem = this._knownUniques.FirstOrDefault(u => u.Name == name);
                        if (uniqueItem != null)
                        {
                            build.AddItem(uniqueItem);
                        }
                    }
                }
            }

            return build;
        }

        private static string GetXml(string build)
        {
            using (var output = new MemoryStream())
            {
                using (var input = new MemoryStream(Convert.FromBase64String(build.Replace("_", "/").Replace("-", "+"))))
                {
                    using (var decompressor = new GZipStream(input, CompressionMode.Decompress))
                    {
                        decompressor.CopyTo(output);
                        return Encoding.UTF8.GetString(output.ToArray());
                    }
                }
            }
        }

        #endregion
    }
}