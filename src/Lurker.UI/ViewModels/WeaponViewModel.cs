//-----------------------------------------------------------------------
// <copyright file="WeaponViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represents a weapon.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class WeaponViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Weapon _weapon;
        private System.Action _closeCallback;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponViewModel" /> class.
        /// </summary>
        /// <param name="weapon">The weapon.</param>
        /// <param name="closeCallback">The close callback.</param>
        public WeaponViewModel(Weapon weapon, System.Action closeCallback)
        {
            this._weapon = weapon;
            this._closeCallback = closeCallback;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the physical DPS.
        /// </summary>
        public double PhysicalDps => this._weapon.PhysicalDps;

        /// <summary>
        /// Gets the elemental DPS.
        /// </summary>
        public double ElementalDps => this._weapon.ElementalDps;

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this._closeCallback?.Invoke();
        }

        #endregion
    }
}