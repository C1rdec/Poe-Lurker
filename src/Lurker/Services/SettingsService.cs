//-----------------------------------------------------------------------
// <copyright file="SettingsService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.IO;
    using ConfOxide;
    using Lurker.Helpers;
    using Lurker.Models;

    /// <summary>
    /// The settings service.
    /// </summary>
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

        /// <summary>
        /// Occurs when [on save].
        /// </summary>
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
        /// Gets or sets a value indicating whether [first launch].
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
        /// Gets or sets a value indicating whether the welcome scrren needs to be shown.
        /// </summary>
        public bool ShowWelcome
        {
            get
            {
                return this._settings.ShowWelcome;
            }

            set
            {
                this._settings.ShowWelcome = value;
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
        /// Gets or sets a value indicating whether [incoming trade enabled].
        /// </summary>
        public bool IncomingTradeEnabled
        {
            get
            {
                return this._settings.IncomingTradeEnabled;
            }

            set
            {
                this._settings.IncomingTradeEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [outgoing trade enabled].
        /// </summary>
        public bool OutgoingTradeEnabled
        {
            get
            {
                return this._settings.OutgoingTradeEnabled;
            }

            set
            {
                this._settings.OutgoingTradeEnabled = value;
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
        /// Gets or sets a value indicating whether [map enabled].
        /// </summary>
        public bool MapEnabled
        {
            get
            {
                return this._settings.MapEnabled;
            }

            set
            {
                this._settings.MapEnabled = value;
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
        /// Gets or sets a value indicating whether [alert enabled].
        /// </summary>
        public bool ItemAlertEnabled
        {
            get
            {
                return this._settings.ItemAlertEnabled;
            }

            set
            {
                this._settings.ItemAlertEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [join hideout enabled].
        /// </summary>
        public bool JoinHideoutEnabled
        {
            get
            {
                return this._settings.JoinHideoutEnabled;
            }

            set
            {
                this._settings.JoinHideoutEnabled = value;
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
        /// Gets or sets the alert volume.
        /// </summary>
        public float ItemAlertVolume
        {
            get
            {
                return this._settings.ItemAlertVolume;
            }

            set
            {
                this._settings.ItemAlertVolume = value;
            }
        }

        /// <summary>
        /// Gets or sets the join hideout volume.
        /// </summary>
        public float JoinHideoutVolume
        {
            get
            {
                return this._settings.JoinHideoutVolume;
            }

            set
            {
                this._settings.JoinHideoutVolume = value;
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
        /// Gets or sets the life foreground.
        /// </summary>
        public string LifeForeground
        {
            get
            {
                return this._settings.LifeForeground;
            }

            set
            {
                this._settings.LifeForeground = value;
            }
        }

        /// <summary>
        /// Gets or sets the tradebar scaling.
        /// </summary>
        public double TradebarScaling
        {
            get
            {
                return this._settings.TradebarScaling;
            }

            set
            {
                this._settings.TradebarScaling = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show startup animation].
        /// </summary>
        public bool ShowStartupAnimation
        {
            get
            {
                return this._settings.ShowStartupAnimation;
            }

            set
            {
                this._settings.ShowStartupAnimation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [vulkan renderer].
        /// </summary>
        public bool VulkanRenderer
        {
            get
            {
                return this._settings.VulkanRenderer;
            }

            set
            {
                this._settings.VulkanRenderer = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [build helper].
        /// </summary>
        public bool BuildHelper
        {
            get
            {
                return this._settings.BuildHelper;
            }

            set
            {
                this._settings.BuildHelper = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sold detection].
        /// </summary>
        public bool SoldDetection
        {
            get
            {
                return this._settings.SoldDetection;
            }

            set
            {
                this._settings.SoldDetection = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show release note].
        /// </summary>
        public bool ShowReleaseNote
        {
            get
            {
                return this._settings.ShowReleaseNote;
            }

            set
            {
                this._settings.ShowReleaseNote = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [time line enabled].
        /// </summary>
        public bool TimelineEnabled
        {
            get
            {
                return this._settings.TimelineEnabled;
            }

            set
            {
                this._settings.TimelineEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [build automatic close].
        /// </summary>
        public bool BuildAutoClose
        {
            get
            {
                return this._settings.BuildAutoClose;
            }

            set
            {
                this._settings.BuildAutoClose = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [group by skill].
        /// </summary>
        public bool GroupBySkill
        {
            get
            {
                return this._settings.GroupBySkill;
            }

            set
            {
                this._settings.GroupBySkill = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [synchronize build].
        /// </summary>
        public bool SyncBuild
        {
            get
            {
                return this._settings.SyncBuild;
            }

            set
            {
                this._settings.SyncBuild = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hideout enabled].
        /// </summary>
        public bool HideoutEnabled
        {
            get
            {
                return this._settings.HideoutEnabled;
            }

            set
            {
                this._settings.HideoutEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hideout enabled].
        /// </summary>
        public bool GuildHideoutEnabled
        {
            get
            {
                return this._settings.GuildHideoutEnabled;
            }

            set
            {
                this._settings.GuildHideoutEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the recent league name.
        /// </summary>
        public string RecentLeagueName
        {
            get
            {
                return this._settings.RecentLeagueName;
            }

            set
            {
                this._settings.RecentLeagueName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we ignore already sold items.
        /// </summary>
        public bool IgnoreAlreadySold
        {
            get
            {
                return this._settings.IgnoreAlreadySold;
            }

            set
            {
                this._settings.IgnoreAlreadySold = value;
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
        private string SettingsFolderPath => System.IO.Path.Combine(this.AppDataFolderPath, this.FolderName);

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        private string SettingsFilePath => System.IO.Path.Combine(this.SettingsFolderPath, this.FileName);

        #endregion

        #region Methods

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
        public void Save(bool raiseEvent = true)
        {
            this._settings.WriteJsonFile(this.SettingsFilePath);
            if (raiseEvent)
            {
                this.OnSave?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Creates the default settings.
        /// </summary>
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