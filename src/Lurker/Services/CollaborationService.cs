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

        private static readonly string CollaborationFileUrl = "https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Collaboration/Collaboration";

        #endregion

        #region Methods

        public async Task<Collaboration> GetCollaborationAsync()
        {
            var fileContent = await this.Get(CollaborationFileUrl);
            return JsonConvert.DeserializeObject<Collaboration>(fileContent);
        }

        #endregion
    }
}
