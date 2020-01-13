//-----------------------------------------------------------------------
// <copyright file="EnumParserBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parsers
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class EnumParserBase<T>
    {
        #region Properties

        protected abstract Dictionary<T, string[]> Dictionary { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The currency type</returns>
        public T Parse(string curencyTypeValue)
        {
            var value = curencyTypeValue.ToLower();
            foreach (var currencyType in Dictionary)
            {
                if (currencyType.Value.Any(v => value.Contains(v)))
                {
                    return currencyType.Key;
                }
            }

            return default(T);
        }

        #endregion
    }
}
