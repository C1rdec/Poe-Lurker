//-----------------------------------------------------------------------
// <copyright file="PathOfNinjaService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Lurker.Models.Ninja;

    /// <summary>
    /// Http Service base.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class PathOfNinjaService : HttpServiceBase
    {
        #region Fields

        private static readonly string BaseUrl = "https://poe.ninja";
        private static readonly string DataUrl = $"{BaseUrl}/api/data";
        private static readonly string CurencyUrl = $"{DataUrl}/currencyoverview";
        private static readonly string ItemOverviewUrl = $"{DataUrl}/itemoverview";
        private IEnumerable<ItemLine> _cachedItems;
        private DateTime _lastUpdate;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
        /// <returns>The exalt ratio.</returns>
        /// <param name="league">The league name.</param>
        public async Task<double> GetExaltRationAsync(string league)
        {
            var result = await this.GetAsync<NinjaResult<CurrencyLine>>($"{CurencyUrl}?league={league}&type=Currency&language=en");

            var exaltLine = result.Lines.FirstOrDefault(l => l.CurrencyTypeName == "Exalted Orb");
            if (exaltLine == null)
            {
                return default;
            }

            return exaltLine.ChaosEquivalent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="league">The league name.</param>
        public async Task RefreshCache(string league)
        {
            var items = new List<ItemLine>();
            foreach (var itemType in Enum.GetValues(typeof(ItemType)))
            {
                var result = await this.GetAsync<NinjaResult<ItemLine>>($"{ItemOverviewUrl}?league={league}&type={itemType}");
                items.AddRange(result.Lines);
            }

            this._cachedItems = items;
            this._lastUpdate = DateTime.Now;
        }
    }
}