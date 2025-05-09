﻿namespace PoeLurker.UI.Models;

using System;
using PoeLurker.UI.ViewModels;

/// <summary>
/// Represents the bar context.
/// </summary>
public class TradebarContext
{
    #region Fields

    private readonly Action<OfferViewModel> _remove;
    private readonly Action<OfferViewModel> _addToSoldOffer;
    private readonly Action<OfferViewModel> _addToActiveOffer;
    private readonly Action<OfferViewModel> _setActiveOffer;
    private readonly Action _clearAll;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TradebarContext" /> class.
    /// </summary>
    /// <param name="removeAction">The remove action.</param>
    /// <param name="addToActiveOfferAction">The add to active offer action.</param>
    /// <param name="addToSoldOfferAction">The add to sold offer action.</param>
    /// <param name="setActiveOffer">The set active offer.</param>
    /// <param name="clearAll">The clear offers.</param>
    public TradebarContext(Action<OfferViewModel> removeAction, Action<OfferViewModel> addToActiveOfferAction, Action<OfferViewModel> addToSoldOfferAction, Action<OfferViewModel> setActiveOffer, Action clearAll)
    {
        _remove = removeAction;
        _addToActiveOffer = addToActiveOfferAction;
        _addToSoldOffer = addToActiveOfferAction;
        _setActiveOffer = setActiveOffer;
        _clearAll = clearAll;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Removes the offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    public void RemoveOffer(OfferViewModel offer)
    {
        _remove(offer);
    }

    /// <summary>
    /// Adds to active offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    public void AddToActiveOffer(OfferViewModel offer)
    {
        _addToActiveOffer(offer);
    }

    /// <summary>
    /// Adds to sold offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    public void AddToSoldOffer(OfferViewModel offer)
    {
        _addToSoldOffer(offer);
    }

    /// <summary>
    /// Sets the active offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    public void SetActiveOffer(OfferViewModel offer)
    {
        _setActiveOffer(offer);
    }

    /// <summary>
    /// Clears the offers.
    /// </summary>
    public void ClearAll()
    {
        _clearAll();
    }

    #endregion
}