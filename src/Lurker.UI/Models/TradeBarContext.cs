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

        private Action<OfferViewModel> _remove;
        private Action<OfferViewModel> _addToActiveOffer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradebarContext"/> class.
        /// </summary>
        /// <param name="removeAction">The remove action.</param>
        public TradebarContext(Action<OfferViewModel> removeAction, Action<OfferViewModel> addToActiveOfferAction)
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

        #endregion
    }
}
