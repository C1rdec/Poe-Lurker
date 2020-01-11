//-----------------------------------------------------------------------
// <copyright file="SettingsService.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using ConfOxide;
    using Lurker.Helpers;
    using Lurker.Models;
    using System;
    using System.IO;

    public class SettingsService
    {
        #region Fields

        private static readonly string DefaultStillInterestedMessage = $"Are you still interested in my {TokenHelper.ItemName} listed for {TokenHelper.Price}";
        private static readonly string DefaultSoldMessage = $"I'm sorry, my {TokenHelper.ItemName} has already been sold.";
        private static readonly string DefaultBusyMessage = "I'm busy right now I'll send you a party invite.";
        private static readonly string DefaultThankYouMessage = string.Empty;

        private Settings _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService()
        {
            this._settings = new Settings();
            if (!Directory.Exists(this.SettingsFolderPath))
            {
                Directory.CreateDirectory(this.SettingsFolderPath);
            }

            if (!File.Exists(this.SettingsFilePath))
            {
                using (var file = File.Create(this.SettingsFilePath))
                { 
                }

                // Set default values
                this.BusyMessage = DefaultBusyMessage;
                this.SoldMessage = DefaultSoldMessage;
                this.StillInterestedMessage = DefaultStillInterestedMessage;
                this.ThankYouMessage = DefaultThankYouMessage;
                this.Save();
            }
            else
            {
                this._settings.ReadJsonFile(this.SettingsFilePath);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        private string FolderName => "PoeLurker";

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        private string FileName => "Settings.json";

        /// <summary>
        /// Gets the application data folder path.
        /// </summary>
        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        private string SettingsFolderPath => System.IO.Path.Combine(AppDataFolderPath, FolderName);

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        private string SettingsFilePath => System.IO.Path.Combine(this.SettingsFolderPath, this.FileName);

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        public string BusyMessage
        {
            get
            {
                return this._settings.BusyMessage;
            }

            set
            {
                this._settings.BusyMessage = value;
            }
        }

        public string SoldMessage
        {
            get
            {
                return this._settings.SoldMessage;
            }

            set
            {
                this._settings.SoldMessage = value;
            }
        }

        public string ThankYouMessage
        {
            get
            {
                return this._settings.ThankYouMessage;
            }

            set
            {
                this._settings.ThankYouMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the sill interested message.
        /// </summary>
        public string StillInterestedMessage
        {
            get
            {
                return this._settings.StillInterestedMessage;
            }

            set
            {
                this._settings.StillInterestedMessage = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            this._settings.WriteJsonFile(this.SettingsFilePath);
        }

        #endregion
    }
}
