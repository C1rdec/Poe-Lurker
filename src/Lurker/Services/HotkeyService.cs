//-----------------------------------------------------------------------
// <copyright file="HotkeyService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System.IO;
    using ConfOxide;
    using Lurker.Models;
    using Winook;

    /// <summary>
    /// Represents Hotkey service.
    /// </summary>
    /// <seealso cref="Lurker.Services.ServiceBase" />
    public class HotkeyService : ServiceBase
    {
        #region Fields

        private HotkeySettings _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyService"/> class.
        /// </summary>
        public HotkeyService()
        {
            this._settings = new HotkeySettings();
            if (!File.Exists(this.FilePath))
            {
                using (var file = File.Create(this.FilePath))
                {
                }

                this.Save();
            }
            else
            {
                try
                {
                    this._settings.ReadJsonFile(this.FilePath);
                }
                catch
                {
                    File.Delete(this.FilePath);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the toggle build.
        /// </summary>
        public ushort ToggleBuild
        {
            get
            {
                return this._settings.ToggleBuild;
            }

            set
            {
                this._settings.ToggleBuild = value;
            }
        }

        /// <summary>
        /// Gets or sets the main.
        /// </summary>
        public Hotkey Main
        {
            get
            {
                return this._settings.Main;
            }

            set
            {
                this._settings.Main = value;
            }
        }

        /// <summary>
        /// Gets or sets the invite.
        /// </summary>
        public Hotkey Invite
        {
            get
            {
                return this._settings.Invite;
            }

            set
            {
                this._settings.Invite = value;
            }
        }

        /// <summary>
        /// Gets or sets the open wiki.
        /// </summary>
        public Hotkey OpenWiki
        {
            get
            {
                return this._settings.OpenWiki;
            }

            set
            {
                this._settings.OpenWiki = value;
            }
        }

        /// <summary>
        /// Gets or sets the join guild hiedout.
        /// </summary>
        public Hotkey JoinGuildHideout
        {
            get
            {
                return this._settings.JoinGuildHideout;
            }

            set
            {
                this._settings.JoinGuildHideout = value;
            }
        }

        /// <summary>
        /// Gets or sets the search item.
        /// </summary>
        public Hotkey SearchItem
        {
            get
            {
                return this._settings.SearchItem;
            }

            set
            {
                this._settings.SearchItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the search item.
        /// </summary>
        public Hotkey RemainingMonster
        {
            get
            {
                return this._settings.RemainingMonster;
            }

            set
            {
                this._settings.RemainingMonster = value;
            }
        }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public Hotkey Busy
        {
            get
            {
                return this._settings.Busy;
            }

            set
            {
                this._settings.Busy = value;
            }
        }

        /// <summary>
        /// Gets or sets the dismiss.
        /// </summary>
        public Hotkey Dismiss
        {
            get
            {
                return this._settings.Dismiss;
            }

            set
            {
                this._settings.Dismiss = value;
            }
        }

        /// <summary>
        /// Gets or sets the still interested.
        /// </summary>
        public Hotkey StillInterested
        {
            get
            {
                return this._settings.StillInterested;
            }

            set
            {
                this._settings.StillInterested = value;
            }
        }

        /// <summary>
        /// Gets or sets the trade.
        /// </summary>
        public Hotkey Trade
        {
            get
            {
                return this._settings.Trade;
            }

            set
            {
                this._settings.Trade = value;
            }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        protected override string FileName => "HotKeys.json";

        #endregion

        #region Methods

        /// <summary>
        /// Saves the specified raise event.
        /// </summary>
        public void Save()
        {
            this._settings.WriteJsonFile(this.FilePath);
        }

        #endregion
    }
}