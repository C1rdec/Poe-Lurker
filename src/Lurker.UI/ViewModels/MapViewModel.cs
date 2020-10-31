//-----------------------------------------------------------------------
// <copyright file="MapViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Linq;
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represents a map item.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class MapViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private const string ReflectPhysicalId = "stat_3464419871";
        private const string ReflectElementalId = "stat_2764017512";
        private const string CannotRegenerateId = "stat_1910157106";
        private const string CannotLeechId = "stat_526251910";
        private const string TemporalChainsId = "stat_2326202293";
        private System.Action _closeCallBack;
        private Map _map;
        private bool _reflectPhysical;
        private bool _reflectElemental;
        private bool _cannotRegenerate;
        private bool _cannotLeech;
        private bool _temporalChains;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel" /> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="closeCallback">The close callback.</param>
        public MapViewModel(Map map, System.Action closeCallback)
        {
            this._map = map;
            this._closeCallBack = closeCallback;
            foreach (var affix in this._map.DangerousAffixes.Where(d => d != null))
            {
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

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this._closeCallBack?.Invoke();
        }

        /// <summary>
        /// Notifies the change.
        /// </summary>
        public void NotifyChange()
        {
            this.NotifyOfPropertyChange(() => this.ReflectPhysical);
            this.NotifyOfPropertyChange(() => this.ReflectElemental);
            this.NotifyOfPropertyChange(() => this.CannotRegenerate);
            this.NotifyOfPropertyChange(() => this.CannotLeech);
            this.NotifyOfPropertyChange(() => this.TemporalChains);
        }

        #endregion
    }
}