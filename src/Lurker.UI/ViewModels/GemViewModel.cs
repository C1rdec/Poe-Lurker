//-----------------------------------------------------------------------
// <copyright file="GemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Lurker.Models;

    /// <summary>
    /// Represents the view model of the gem.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class GemViewModel : WikiItemBaseViewModel
    {
        #region Fields

        private static readonly Uri DefaultGemImage = new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/3/31/Portal_inventory_icon.png/revision/latest?cb=20130626162348");
        private Gem _gem;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GemViewModel" /> class.
        /// </summary>
        /// <param name="gem">The gem.</param>
        /// <param name="showLevel">if set to <c>true</c> [show level].</param>
        public GemViewModel(Gem gem, bool showLevel = true)
            : base(gem)
        {
            this._gem = gem;
            this.ShowLevel = showLevel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default image.
        /// </summary>
        public override Uri DefaultImage => DefaultGemImage;

        /// <summary>
        /// Gets a value indicating whether this <see cref="GemViewModel"/> is support.
        /// </summary>
        public bool Support => this._gem.Support;

        /// <summary>
        /// Gets a value indicating whether [show level].
        /// </summary>
        public bool ShowLevel { get; private set; }

        /// <summary>
        /// Gets the gem location.
        /// </summary>
        public GemLocationViewModel GemLocation => this._gem.Location != null ? new GemLocationViewModel(this._gem.Location) : null;

        #endregion
    }
}