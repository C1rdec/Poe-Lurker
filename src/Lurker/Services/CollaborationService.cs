//-----------------------------------------------------------------------
// <copyright file="CollaborationService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Threading.Tasks;
    using Lurker.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the collaboation Service.
    /// </summary>
    /// <seealso cref="Lurker.Services.HttpServiceBase" />
    public class CollaborationService : HttpServiceBase
    {
        #region Fields

        private static readonly string BaseGitUrl = "https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Collaboration/";
        private static readonly string InformationFileUrl = $"{BaseGitUrl}Info";
        private static readonly string ImageFileUrl = $"{BaseGitUrl}Image";

        #endregion

        #region Methods

        /// <summary>
        /// Gets the collaboration asynchronous.
        /// </summary>
        /// <returns>The collaboration.</returns>
        public async Task<Collaboration> GetCollaborationAsync()
        {
            try
            {
                var fileContent = await this.GetText(InformationFileUrl);
                return JsonConvert.DeserializeObject<Collaboration>(fileContent);
            }
            catch
            {
                return new Collaboration()
                {
                    ExpireDate = DateTime.Now.AddDays(-1),
                };
            }
        }

        /// <summary>
        /// Gets the image asynchronous.
        /// </summary>
        /// <returns>The image content.</returns>
        public Task<byte[]> GetImageAsync()
        {
            return this.GetContent(ImageFileUrl);
        }

        #endregion
    }
}