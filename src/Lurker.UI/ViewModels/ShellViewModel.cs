//-----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI 
{
    using Lurker.UI.ViewModels;
    using System.Collections.ObjectModel;

    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell 
    {
        #region Fields

        private ClientLurker _Lurker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            this.TradeOffers = new ObservableCollection<TradeOfferViewModel>();
            this._Lurker = new ClientLurker();
            this._Lurker.NewOffer += this._Lurker_NewOffer;
        }

        /// <summary>
        /// Lurkers the new offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The trade event.</param>
        private void _Lurker_NewOffer(object sender, Events.TradeEvent e)
        {
            this.TradeOffers.Add(new TradeOfferViewModel(e));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the trade offers.
        /// </summary>
        public ObservableCollection<TradeOfferViewModel> TradeOffers { get; set; }

        #endregion
    }
}