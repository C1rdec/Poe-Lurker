//-----------------------------------------------------------------------
// <copyright file="OutgoingbarContext.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Models;

using System;
using PoeLurker.UI.ViewModels;

/// <summary>
/// Represents the bar context.
/// </summary>
public class OutgoingbarContext
{
    #region Fields

    private readonly Action<OutgoingOfferViewModel> _remove;
    private readonly Action<OutgoingOfferViewModel> _setActiveOffer;
    private readonly Action _clearAll;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OutgoingbarContext" /> class.
    /// </summary>
    /// <param name="removeAction">The remove action.</param>
    /// <param name="setActiveOffer">The set active offer.</param>
    /// <param name="clearAll">The clear all.</param>
    public OutgoingbarContext(Action<OutgoingOfferViewModel> removeAction, Action<OutgoingOfferViewModel> setActiveOffer, Action clearAll)
    {
        _remove = removeAction;
        _setActiveOffer = setActiveOffer;
        _clearAll = clearAll;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Removes the offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    public void RemoveOffer(OutgoingOfferViewModel offer)
    {
        _remove(offer);
    }

    /// <summary>
    /// Sets the active offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    public void SetActiveOffer(OutgoingOfferViewModel offer)
    {
        _setActiveOffer(offer);
    }

    /// <summary>
    /// Clears a ll.
    /// </summary>
    public void ClearAll()
    {
        _clearAll();
    }

    #endregion
}