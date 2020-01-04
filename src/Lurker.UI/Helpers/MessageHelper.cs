//-----------------------------------------------------------------------
// <copyright file="MessageHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represent a helper class that regroup all the message of the application.
    /// </summary>
    public static class MessageHelper
    {
        #region Fields

        private static readonly Lazy<string> BusyMessageLazy = new Lazy<string>(() => GetSettingValue("BusyMessage", "I'm busy right now I'll send you a party invite."));
        private static readonly Lazy<string> SoldMessageLazy = new Lazy<string>(() => GetSettingValue("SoldMessage", "I'm sorry, that item has already been sold."));
        private static readonly Lazy<string> ThanksMessageLazy = new Lazy<string>(() => GetSettingValue("ThanksMessage", string.Empty));

        #endregion

        #region Properties

        /// <summary>
        /// Gets the busy message.
        /// </summary>
        public static string BusyMessage => BusyMessageLazy.Value;

        /// <summary>
        /// Gets the sold message.
        /// </summary>
        public static string SoldMessage => SoldMessageLazy.Value;

        /// <summary>
        /// Gets the thanks message.
        /// </summary>
        public static string ThanksMessage => ThanksMessageLazy.Value;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The setting value</returns>
        private static string GetSettingValue(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return value;
        }

        #endregion
    }
}
