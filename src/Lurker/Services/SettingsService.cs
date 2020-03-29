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

        private static readonly string DefaultStillInterestedMessage = $"Are you still interested in my {TokenHelper.ItemName} listed for {TokenHelper.Price}?";
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
                this.CreateDefaultSettings();
            }
            else
            {
                try
                {
                    this._settings.ReadJsonFile(this.SettingsFilePath);
                }
                catch
                {
                    File.Delete(this.SettingsFilePath);
                    this.CreateDefaultSettings();
                }
            }

            if (string.IsNullOrEmpty(this._settings.UserId))
            {
                this._settings.UserId = Guid.NewGuid().ToString();
                this.Save();
            }
        }

        #endregion

        #region Events

        public event EventHandler OnSave;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        public string UserId => this._settings.UserId;

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

        /// <summary>
        /// Gets or sets the sold message.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the thank you message.
        /// </summary>
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
        /// Gets or sets the first launch.
        /// </summary>
        public bool FirstLaunch
        {
            get
            {
                return this._settings.FirstLaunch;
            }

            set
            {
                this._settings.FirstLaunch = value;
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

        /// <summary>
        /// Gets or sets a value indicating whether [search enabled].
        /// </summary>
        public bool SearchEnabled
        {
            get
            {
                return this._settings.SearchEnabled;
            }

            set
            {
                this._settings.SearchEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug enabled].
        /// </summary>
        public bool DebugEnabled
        {
            get
            {
                return this._settings.DebugEnabled;
            }

            set
            {
                this._settings.DebugEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [alert enabled].
        /// </summary>
        public bool AlertEnabled
        {
            get
            {
                return this._settings.AlertEnabled;
            }

            set
            {
                this._settings.AlertEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [tool tip enabled].
        /// </summary>
        public bool ToolTipEnabled
        {
            get
            {
                return this._settings.ToolTipEnabled;
            }

            set
            {
                this._settings.ToolTipEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [clipboard enabled].
        /// </summary>
        public bool ClipboardEnabled
        {
            get
            {
                return this._settings.ClipboardEnabled;
            }

            set
            {
                this._settings.ClipboardEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic kick enabled].
        /// </summary>
        public bool AutoKickEnabled
        {
            get
            {
                return this._settings.AutoKickEnabled;
            }

            set
            {
                this._settings.AutoKickEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [item highlight enabled].
        /// </summary>
        public bool ItemHighlightEnabled
        {
            get
            {
                return this._settings.ItemHighlightEnabled;
            }

            set
            {
                this._settings.ItemHighlightEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [remaining monster enabled].
        /// </summary>
        public bool RemainingMonsterEnabled
        {
            get
            {
                return this._settings.RemainingMonsterEnabled;
            }

            set
            {
                this._settings.RemainingMonsterEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dashboard enabled].
        /// </summary>
        public bool DashboardEnabled
        {
            get
            {
                return this._settings.DashboardEnabled;
            }

            set
            {
                this._settings.DashboardEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the alert volume.
        /// </summary>
        public float AlertVolume
        {
            get
            {
                return this._settings.AlertVolume;
            }

            set
            {
                this._settings.AlertVolume = value;
            }
        }

        /// <summary>
        /// Gets or sets the tool tip delay.
        /// </summary>
        public int ToolTipDelay
        {
            get
            {
                return this._settings.ToolTipDelay;
            }

            set
            {
                this._settings.ToolTipDelay = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hide in background].
        /// </summary>
        public bool HideInBackground
        {
            get
            {
                return this._settings.HideInBackground;
            }

            set
            {
                this._settings.HideInBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [delete item enabled].
        /// </summary>
        public bool DeleteItemEnabled
        {
            get
            {
                return this._settings.DeleteItemEnabled;
            }

            set
            {
                this._settings.DeleteItemEnabled = value;
            }
        }

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

        #endregion

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            this._settings.WriteJsonFile(this.SettingsFilePath);
            this.OnSave?.Invoke(this, EventArgs.Empty);
        }

        private void CreateDefaultSettings()
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

        #endregion
    }
}
