//-----------------------------------------------------------------------
// <copyright file="TradeAPIClient.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Models.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lurker.Models.TradeAPI
{
    public class TradeApiClient
    {
        #region Fields

        private static readonly string BaseApiUrl = "https://www.pathofexile.com/api/trade/data";
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        #endregion

        #region Methods

        public async Task<IEnumerable<AffixEntry>> GetAffixes()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{BaseApiUrl}/stats");
                var stringContent = await response.Content.ReadAsStringAsync();

                var queryResult = JsonConvert.DeserializeObject<QueryResult<AffixCategory>>(stringContent, SerializerSettings);

                var affixEntries = new List<AffixEntry>();
                foreach (var result in queryResult.Result)
                {
                    affixEntries.AddRange(result.Entries);
                }

                return affixEntries;
            }
        }

        #endregion
    }
}
