//-----------------------------------------------------------------------
// <copyright file="TradeBarContext.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using Lurker.UI.ViewModels;
    using System;

    public class TradebarContext
    {
        #region Fields

        private Action<TradeOfferViewModel> _remove;
        private Action<TradeOfferViewModel> _addToActiveOffer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradebarContext"/> class.
        /// </summary>
        /// <param name="removeAction">The remove action.</param>
        public TradebarContext(Action<TradeOfferViewModel> removeAction, Action<TradeOfferViewModel> addToActiveOfferAction)
        {
            this._remove = removeAction;
            this._addToActiveOffer = addToActiveOfferAction;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void RemoveOffer(TradeOfferViewModel offer)
        {
            this._remove(offer);
        }

        /// <summary>
        /// Adds to active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void AddToActiveOffer(TradeOfferViewModel offer)
        {
            this._addToActiveOffer(offer);
        }

        #endregion
    }
}
