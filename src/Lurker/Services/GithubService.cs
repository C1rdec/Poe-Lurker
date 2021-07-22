//-----------------------------------------------------------------------
// <copyright file="GithubService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Lurker.Models;

    /// <summary>
    /// Represents the Github Service.
    /// </summary>
    /// <seealso cref="Lurker.Services.HttpServiceBase" />
    public class GithubService : HttpServiceBase
    {
        private IEnumerable<Gem> _gems;
        private IEnumerable<UniqueItem> _items;

        /// <summary>
        /// Gets all items.
        /// </summary>
        public IEnumerable<WikiItem> AllItems => this._gems.Concat<WikiItem>(this._items);

        /// <summary>
        /// Gemses this instance.
        /// </summary>
        /// <returns>List of Gems.</returns>
        public async Task<IEnumerable<Gem>> Gems()
        {
            if (this._gems == null)
            {
                var gemInformation = await this.GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Data/GemInfo.json?{Guid.NewGuid()}");
                this._gems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Gem>>(gemInformation);
            }

            return this._gems;
        }

        /// <summary>
        /// Uniqueses this instance.
        /// </summary>
        /// <returns>List of uniques.</returns>
        public async Task<IEnumerable<UniqueItem>> Uniques()
        {
            if (this._items == null)
            {
                var uniqueInformation = await this.GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Data/UniqueInfo.json?{Guid.NewGuid()}");
                this._items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UniqueItem>>(uniqueInformation);
            }

            return this._items;
        }

        /// <summary>
        /// Searches the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The list of items.</returns>
        public IEnumerable<WikiItem> Search(string value)
        {
            if (string.IsNullOrEmpty(value) || this._items == null || this._gems == null)
            {
                return Enumerable.Empty<WikiItem>();
            }

            value = value.ToLower();
            return this.AllItems.Where(i => i.Name.ToLower().Contains(value)).Take(10);
        }
    }
}