//-----------------------------------------------------------------------
// <copyright file="HttpServiceBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class HttpServiceBase : IDisposable
    {
        #region Fields

        private HttpClient _client;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceBase"/> class.
        /// </summary>
        protected HttpServiceBase()
        {
            this._client = new HttpClient();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this._client.Dispose();
            }
        }

        /// <summary>
        /// Creates the authorize URL.
        /// </summary>
        protected async Task<string> GetText(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await this._client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="url">The URL.</param>
        protected async Task<byte[]> GetContent(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await this._client.SendAsync(request);
            return await response.Content.ReadAsByteArrayAsync();
        }

        #endregion
    }
}
