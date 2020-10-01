//-----------------------------------------------------------------------
// <copyright file="GemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;
    using Lurker.Models;

    /// <summary>
    /// Represents the view model of the gem.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class GemViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private static readonly Uri DefaultImage = new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/3/31/Portal_inventory_icon.png/revision/latest?cb=20130626162348");
        private Gem _gem;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GemViewModel"/> class.
        /// </summary>
        /// <param name="gem">The gem.</param>
        public GemViewModel(Gem gem)
        {
            this._gem = gem;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the wiki URL.
        /// </summary>
        public string Name => this._gem.Name;

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level => this._gem.Level;

        /// <summary>
        /// Gets the wiki URL.
        /// </summary>
        public Uri WikiUrl => this._gem.WikiUrl;

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        public Uri ImageUrl => this._gem.ImageUrl ?? DefaultImage;

        /// <summary>
        /// Gets a value indicating whether this <see cref="GemViewModel"/> is support.
        /// </summary>
        public bool Support => this._gem.Support;

        /// <summary>
        /// Gets the gem location.
        /// </summary>
        public GemLocationViewModel GemLocation => this._gem.Location != null ? new GemLocationViewModel(this._gem.Location) : null;

        #endregion

        #region Methods

        /// <summary>
        /// Opens the wiki.
        /// </summary>
        public void OpenWiki()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Process.Start(this.WikiUrl.ToString());
            }
        }

        #endregion
    }
}