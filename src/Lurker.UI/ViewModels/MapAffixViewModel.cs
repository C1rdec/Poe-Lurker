﻿//-----------------------------------------------------------------------
// <copyright file="MapAffixViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Linq;
    using Lurker.Patreon;
    using Lurker.Patreon.Models;

    /// <summary>
    /// `Represents a dangerous map affix.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class MapAffixViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        /// <summary>
        /// The reflect physical identifier.
        /// </summary>
        public const string ReflectPhysicalId = "stat_3464419871";

        /// <summary>
        /// The reflect elemental identifier.
        /// </summary>
        public const string ReflectElementalId = "stat_2764017512";

        /// <summary>
        /// The cannot regenerate identifier.
        /// </summary>
        public const string CannotRegenerateId = "stat_1910157106";

        /// <summary>
        /// The cannot leech identifier.
        /// </summary>
        public const string CannotLeechId = "stat_526251910";

        /// <summary>
        /// The temporal chains identifier.
        /// </summary>
        public const string TemporalChainsId = "stat_2326202293";

        private string _id;
        private PlayerViewModel _playerViewModel;
        private Affix _affix;
        private bool _reflectPhysical;
        private bool _reflectElemental;
        private bool _cannotRegenerate;
        private bool _cannotLeech;
        private bool _temporalChains;
        private bool _helpVisible;
        private bool _selected;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapAffixViewModel" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="selectable">if set to <c>true</c> [selectable].</param>
        /// <param name="playerViewModel">The player view model.</param>
        public MapAffixViewModel(string id, bool selectable, PlayerViewModel playerViewModel)
        {
            this.Selectable = selectable;
            switch (id)
            {
                case ReflectPhysicalId:
                    this._reflectPhysical = true;
                    break;
                case ReflectElementalId:
                    this._reflectElemental = true;
                    break;
                case CannotRegenerateId:
                    this._cannotRegenerate = true;
                    break;
                case CannotLeechId:
                    this._cannotLeech = true;
                    break;
                case TemporalChainsId:
                    this._temporalChains = true;
                    break;
            }

            this._id = id;
            this._playerViewModel = playerViewModel;
            this._selected = !this._playerViewModel.IgnoredMadMods.Contains(id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapAffixViewModel" /> class.
        /// </summary>
        /// <param name="affix">The affix.</param>
        /// <param name="playerViewModel">The player view model.</param>
        public MapAffixViewModel(Affix affix, PlayerViewModel playerViewModel)
            : this(affix.Id, false, playerViewModel)
        {
            this._affix = affix;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="MapAffixViewModel"/> is selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                return this._selected;
            }

            private set
            {
                this._selected = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [popup visible].
        /// </summary>
        public bool HelpVisible
        {
            get
            {
                return this._helpVisible;
            }

            private set
            {
                this._helpVisible = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [reflect physical].
        /// </summary>
        public bool ReflectPhysical => this._reflectPhysical;

        /// <summary>
        /// Gets a value indicating whether [reflect elemental].
        /// </summary>
        public bool ReflectElemental => this._reflectElemental;

        /// <summary>
        /// Gets a value indicating whether [cannot regenerate].
        /// </summary>
        public bool CannotRegenerate => this._cannotRegenerate;

        /// <summary>
        /// Gets a value indicating whether [cannot leech].
        /// </summary>
        public bool CannotLeech => this._cannotLeech;

        /// <summary>
        /// Gets a value indicating whether [temporal chains].
        /// </summary>
        public bool TemporalChains => this._temporalChains;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => this._affix == null ? string.Empty : this._affix.Text;

        /// <summary>
        /// Gets a value indicating whether this <see cref="MapAffixViewModel"/> is selectable.
        /// </summary>
        public bool Selectable { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        public void OnClick()
        {
            if (!this.Selectable)
            {
                return;
            }

            if (this.Selected)
            {
                this._playerViewModel.AddIgnoredMapMod(this._id);
            }
            else
            {
                this._playerViewModel.RemoveIgnoredMapMod(this._id);
            }

            this.Selected = !this.Selected;
        }

        /// <summary>
        /// Mouses the enter.
        /// </summary>
        public void MouseEnter()
        {
            if (!this.Selectable)
            {
                this.HelpVisible = true;
            }
        }

        /// <summary>
        /// Mouses the leave.
        /// </summary>
        public void MouseLeave()
        {
            this.HelpVisible = false;
        }

        #endregion
    }
}