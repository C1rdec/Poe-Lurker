//-----------------------------------------------------------------------
// <copyright file="PathOfNinjaService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
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
        private static readonly string CurencyUrl = $"{BaseUrl}/api/data/currencyoverview";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService"/> class.
        /// </summary>
        /// <returns>The exalt ratio.</returns>
        /// <param name="league">The league name.</param>
        public async Task<double> GetExaltRationAsync(string league)
        {
            var result = await this.GetAsync<NinjaResult>($"{CurencyUrl}?league={league}&type=Currency&language=en");

            var exaltLine = result.Lines.FirstOrDefault(l => l.CurrencyTypeName == "Exalted Orb");
            if (exaltLine == null)
            {
                return default;
            }

            return exaltLine.ChaosEquivalent;
        }
    }
}