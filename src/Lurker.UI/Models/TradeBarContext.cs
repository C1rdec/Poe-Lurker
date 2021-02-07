//-----------------------------------------------------------------------
// <copyright file="TradebarContext.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using System;
    using Lurker.UI.ViewModels;

    /// <summary>
    /// Represents the bar context.
    /// </summary>
    public class TradebarContext
    {
        #region Fields

        private Action<OfferViewModel> _remove;
        private Action<OfferViewModel> _addToSoldOffer;
        private Action<OfferViewModel> _addToActiveOffer;
        private Action<OfferViewModel> _setActiveOffer;
        private Action _clearAll;

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
            this._remove = removeAction;
            this._addToActiveOffer = addToActiveOfferAction;
            this._addToSoldOffer = addToActiveOfferAction;
            this._setActiveOffer = setActiveOffer;
            this._clearAll = clearAll;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void RemoveOffer(OfferViewModel offer)
        {
            this._remove(offer);
        }

        /// <summary>
        /// Adds to active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void AddToActiveOffer(OfferViewModel offer)
        {
            this._addToActiveOffer(offer);
        }

        /// <summary>
        /// Adds to sold offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void AddToSoldOffer(OfferViewModel offer)
        {
            this._addToSoldOffer(offer);
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void SetActiveOffer(OfferViewModel offer)
        {
            this._setActiveOffer(offer);
        }

        /// <summary>
        /// Clears the offers.
        /// </summary>
        public void ClearAll()
        {
            this._clearAll();
        }

        #endregion
    }
}