//-----------------------------------------------------------------------
// <copyright file="PathOfBuildingService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Lurker.Models;

    /// <summary>
    /// Represents the service for Path ofBuilding.
    /// </summary>
    public class PathOfBuildingService
    {
        #region Methods

        /// <summary>
        /// Decodes the specified build.
        /// </summary>
        /// <param name="buildValue">The build value.</param>
        /// <returns>
        /// The xml structure.
        /// </returns>
        public static Build Decode(string buildValue)
        {
            var build = new Build();
            var document = XDocument.Parse(GetXml(buildValue));
            var skillsElement = document.Root.Element("Skills");
            if (skillsElement != null)
            {
                foreach (var element in skillsElement.Elements())
                {
                    var skill = Skill.FromXml(element);
                    if (skill.Gems.Any())
                    {
                        build.AddSkill(skill);
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