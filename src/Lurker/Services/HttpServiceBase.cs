//-----------------------------------------------------------------------
// <copyright file="HttpServiceBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Http Service base.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public abstract class HttpServiceBase : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceBase"/> class.
        /// </summary>
        protected HttpServiceBase()
        {
            this.Client = new HttpClient();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        protected HttpClient Client { get; set; }

        #endregion

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
                this.Client.Dispose();
            }
        }

        /// <summary>
        /// Creates the authorize URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The text.</returns>
        protected async Task<string> GetText(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await this.Client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The byte­[].</returns>
        protected async Task<byte[]> GetContent(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await this.Client.SendAsync(request);
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <typeparam name="T">The type of the reponse.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>The byte­[].</returns>
        protected async Task<T> GetAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await this.Client.SendAsync(request);
            var value = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(value);
        }

        #endregion
    }
}