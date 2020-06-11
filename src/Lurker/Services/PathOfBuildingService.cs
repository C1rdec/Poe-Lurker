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
    using System.Text;

    /// <summary>
    /// Represents the service for Path ofBuilding
    /// </summary>
    public class PathOfBuildingService
    {
        #region Methods

        /// <summary>
        /// Decodes the specified build.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>The xml structure.</returns>
        public static string Decode(string build)
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