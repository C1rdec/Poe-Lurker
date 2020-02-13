//-----------------------------------------------------------------------
// <copyright file="EnumParserBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parser
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
        public T Parse(string value)
        {
            foreach (var dictionaryValue in Dictionary)
            {
                if (dictionaryValue.Value.Any(v => this.Compare(value, v)))
                {
                    return dictionaryValue.Key;
                }
            }

            return default;
        }

        protected virtual bool Compare(string value, string dictionnaryValue) => value.ToLower().Contains(dictionnaryValue);

        #endregion
    }
}
