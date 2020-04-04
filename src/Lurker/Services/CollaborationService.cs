//-----------------------------------------------------------------------
// <copyright file="CollaborationService.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using Lurker.Models;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

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
        /// <returns></returns>
        public async Task<Collaboration> GetCollaborationAsync()
        {
            var fileContent = await this.GetText(InformationFileUrl);
            return JsonConvert.DeserializeObject<Collaboration>(fileContent);
        }

        /// <summary>
        /// Gets the image asynchronous.
        /// </summary>
        /// <returns>The image content</returns>
        public async Task<byte[]> GetImageAsync()
        {
            return await this.GetContent(ImageFileUrl);
        }

        #endregion
    }
}
