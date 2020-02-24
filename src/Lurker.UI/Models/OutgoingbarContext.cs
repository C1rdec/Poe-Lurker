//-----------------------------------------------------------------------
// <copyright file="OutgoingbarContext.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using Lurker.UI.ViewModels;
    using System;

    public class OutgoingbarContext
    {
        #region Fields

        private Action<OutgoingOfferViewModel> _remove;
        private Action<OutgoingOfferViewModel> _setActiveOffer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingbarContext"/> class.
        /// </summary>
        /// <param name="removeAction">The remove action.</param>
        public OutgoingbarContext(Action<OutgoingOfferViewModel> removeAction, Action<OutgoingOfferViewModel> setActiveOffer)
        {
            this._remove = removeAction;
            this._setActiveOffer = setActiveOffer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void RemoveOffer(OutgoingOfferViewModel offer)
        {
            this._remove(offer);
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        public void SetActiveOffer(OutgoingOfferViewModel offer)
        {
            this._setActiveOffer(offer);
        }

        #endregion
    }
}
