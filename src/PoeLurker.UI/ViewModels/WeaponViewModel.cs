//-----------------------------------------------------------------------
// <copyright file="WeaponViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using PoeLurker.Patreon.Models;

/// <summary>
/// Represents a weapon.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class WeaponViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly WeaponBase _weapon;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaponViewModel" /> class.
    /// </summary>
    /// <param name="weapon">The weapon.</param>
    public WeaponViewModel(WeaponBase weapon)
    {
        _weapon = weapon;

        if (PhysicalDps >= ElementalDps)
        {
            HigherPhysicalDammage = true;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the progress.
    /// </summary>
    public double Progress => (_weapon.QualityPhysicalDps * Width) / _weapon.MaxPhysicalDps;

    /// <summary>
    /// Gets the width.
    /// </summary>
    public int Width => 150;

    /// <summary>
    /// Gets a value indicating whether [higher physical dammage].
    /// </summary>
    public bool HigherPhysicalDammage { get; private set; }

    /// <summary>
    /// Gets a value indicating whether [higher elemental dammage].
    /// </summary>
    public bool HigherElementalDammage => !HigherPhysicalDammage;

    /// <summary>
    /// Gets the physical DPS.
    /// </summary>
    public double PhysicalDps => _weapon.QualityPhysicalDps;

    /// <summary>
    /// Gets the elemental DPS.
    /// </summary>
    public double ElementalDps => _weapon.ElementalDps;

    #endregion
}