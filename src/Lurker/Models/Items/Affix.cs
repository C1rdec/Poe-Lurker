//-----------------------------------------------------------------------
// <copyright file="Affix.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Models.Items
{
    using Lurker.Services;
    using System.Collections.Generic;
    using System.Linq;

    public class Affix
    {
        #region Fields

        private string _actualValue;
        private string _text;
        private string _id;
        private double _value;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Affix"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Affix(string value)
        {
            this._actualValue = value;
            this.Format(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id => this._id;

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text => this._text;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value => this._value;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="textValue">The text value.</param>
        /// <returns>The affix id.</returns>
        private static string GetId(string textValue)
        {
            if (textValue.EndsWith(AffixService.CraftedMarker))
            {
                return AffixService.FindCraftedId(textValue);
            }

            if (textValue.EndsWith(AffixService.ImplicitMarker))
            {
                return AffixService.FindImplicitId(textValue);
            }

            return AffixService.FindExplicitId(textValue);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Format(string value)
        {
            var digitValues = GetDigitStrings(value);
            foreach (var digitValue in digitValues)
            {
                value = value.Replace(digitValue, "#");
            }

            var text = value.Replace("+", string.Empty);
            this._id = GetId(text);

            // Remove (Crafted) | (Implicit) markers
            this._text = AffixService.SimplifyText(text);
            if (digitValues.Any())
            {
                this._value = digitValues.Select(d => double.Parse(d)).Average();
            }
        }

        /// <summary>
        /// Gets the digit strings.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static IEnumerable<string> GetDigitStrings(string value)
        {
            var results = new List<string>();
            var currentValue = string.Empty;
            foreach (var character in value)
            {
                if (char.IsDigit(character) || character == '.')
                {
                    currentValue += character;
                }
                else if (!string.IsNullOrEmpty(currentValue))
                {
                    results.Add(currentValue);
                    currentValue = string.Empty;
                }
            }

            return results;
        }

        #endregion
    }
}
