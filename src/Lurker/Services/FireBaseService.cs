//-----------------------------------------------------------------------
// <copyright file="FireBaseService.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Lurker.Models;
using Lurker.Patreon.Events;
using Lurker.Patreon.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lurker.Services
{
    public class FireBaseService : HttpServiceBase
    {
        #region Fields

        private static readonly string BaseUrl = "https://fcm.googleapis.com/fcm/send";

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public async Task SendMessageAsync(string message)
        {
            var credential = GoogleCredential.GetApplicationDefault();
            FirebaseAdmin.FirebaseApp.Create();
            var message2 = new Message()
            {
                Token = "c_QIkJm668Q:APA91bEN_AqY47lBMaK-rN6e7fk8r7Ey_tkUyRux3SeMVmlrNEkmBB7r6ujil5h-llJCV9itr5KfsPQjIIkhuXAnj_SBdSV_20gGtZsgPd2u1OtU4VO16aEDZE8WxAkgt8HnoASoma6l",
                Data = new Dictionary<string, string>() { { "CurrencyType", CurrencyType.Chaos.ToString() } , { "NumberOfCurrencies", "69" } } 
            };
            await FirebaseMessaging.DefaultInstance.SendAsync(message2);

            var request = new FBCMRequest()
            {
                To = "c_QIkJm668Q:APA91bEN_AqY47lBMaK-rN6e7fk8r7Ey_tkUyRux3SeMVmlrNEkmBB7r6ujil5h-llJCV9itr5KfsPQjIIkhuXAnj_SBdSV_20gGtZsgPd2u1OtU4VO16aEDZE8WxAkgt8HnoASoma6l",
                Data = new SimpleTradeModel() { Price = new Price() { CurrencyType = CurrencyType.Chaos, NumberOfCurrencies = 69 } },
            };

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var json = JsonConvert.SerializeObject(request, serializerSettings);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await this._client.PostAsync(BaseUrl, data);
        }

        public async Task Authenticate()
        {
            var apiKey = "AIzaSyBOwZja1ky4eINiF7Ma0QO5q6d3pLXuZyA";
            var cloundMessagingToken = "AAAAnYEYm-0:APA91bFPe6B0P7WDA-rpfoidaj1UK8z-TniWjO7fDthYTb_ezXNc0UTI_q0ZYRzFlOKmVtSiJjNkq_wp66fsXd1lm2EhxDVBxVAFGwBODPNnPsJSdC8nPDoIk23Jr1wk6h-uDkjvGjkU";
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={apiKey}";


            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var json = JsonConvert.SerializeObject(new { token = cloundMessagingToken, returnSecureToken = true}, serializerSettings);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await this._client.PostAsync(url, data);
        }

        #endregion
    }
}
