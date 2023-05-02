//-----------------------------------------------------------------------
// <copyright file="WikiItemBaseViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Represents a wiki item.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public abstract class WikiItemBaseViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private WikiItem _item;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiItemBaseViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public WikiItemBaseViewModel(WikiItem item)
        {
            this._item = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the wiki URL.
        /// </summary>
        public string Name => this._item.Name;

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level => this._item.Level;

        /// <summary>
        /// Gets the wiki URL.
        /// </summary>
        public Uri WikiUrl => this._item.WikiUrl;

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        public Uri ImageUrl => this._item.ImageUrl ?? this.DefaultImage;

        /// <summary>
        /// Gets the default image.
        /// </summary>
        public abstract Uri DefaultImage { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the wiki.
        /// </summary>
        public void OpenWiki()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var league = IoC.Get<SettingsService>().RecentLeagueName;
                if (this._item is Gem || string.IsNullOrEmpty(league))
                {
                    Process.Start(this.WikiUrl.ToString());

                    return;
                }

                var baseQuery = $"{{ \"query\": {{ \"status\": {{ \"option\": \"online\" }}, \"name\": \"{this.Name}\" }}, \"sort\": {{ \"price\": \"asc\" }} }}";

                var encoded = HttpUtility.UrlEncode(baseQuery);
                var search = $"https://www.pathofexile.com/trade/search/{league}?q={encoded}";
                Process.Start(search);
            }
        }

        #endregion
    }
}