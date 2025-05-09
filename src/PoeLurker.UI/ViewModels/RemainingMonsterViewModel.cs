﻿//-----------------------------------------------------------------------
// <copyright file="RemainingMonsterViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using PoeLurker.Patreon.Events;

/// <summary>
/// Represents the remaining monster.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class RemainingMonsterViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly MonstersRemainEvent _monsterEvent;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RemainingMonsterViewModel"/> class.
    /// </summary>
    /// <param name="monsterEvent">The monster event.</param>
    public RemainingMonsterViewModel(MonstersRemainEvent monsterEvent)
    {
        _monsterEvent = monsterEvent;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the monster count.
    /// </summary>
    public int MonsterCount => _monsterEvent.MonsterCount;

    /// <summary>
    /// Gets a value indicating whether [less than50].
    /// </summary>
    public bool LessThan50 => MonsterCount < 50;

    #endregion
}