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
        public Uri ImageUrl => this._gem.ImageUrl;

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