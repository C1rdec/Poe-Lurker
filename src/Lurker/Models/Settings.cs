//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using ConfOxide;

    public sealed class Settings: SettingsBase<Settings>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        public string BusyMessage { get; set; }

        /// <summary>
        /// Gets or sets the sold message.
        /// </summary>
        public string SoldMessage { get; set; }

        /// <summary>
        /// Gets or sets the thank you message.
        /// </summary>
        public string ThankYouMessage { get; set; }

        /// <summary>
        /// Gets or sets the sill interested message.
        /// </summary>
        public string StillInterestedMessage { get; set; }

        #endregion
    }
}
