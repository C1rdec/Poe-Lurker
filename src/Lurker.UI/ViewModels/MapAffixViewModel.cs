//-----------------------------------------------------------------------
// <copyright file="MapAffixViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Patreon.Models;

    /// <summary>
    /// `Represents a dangerous map affix.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class MapAffixViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private const string ReflectPhysicalId = "stat_3464419871";
        private const string ReflectElementalId = "stat_2764017512";
        private const string CannotRegenerateId = "stat_1910157106";
        private const string CannotLeechId = "stat_526251910";
        private const string TemporalChainsId = "stat_2326202293";
        private Affix _affix;
        private bool _reflectPhysical;
        private bool _reflectElemental;
        private bool _cannotRegenerate;
        private bool _cannotLeech;
        private bool _temporalChains;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapAffixViewModel"/> class.
        /// </summary>
        /// <param name="affix">The affix.</param>
        public MapAffixViewModel(Affix affix)
        {
            this._affix = affix;

            switch (affix.Id)
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
        }

        #endregion

        #region Properties

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
        public string Name => this._affix.Text;

        #endregion
    }
}