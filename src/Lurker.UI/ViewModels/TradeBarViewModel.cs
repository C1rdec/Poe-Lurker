//-----------------------------------------------------------------------
// <copyright file="TradeBarViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.UI.Helpers;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    public class TradeBarViewModel : Screen, IViewAware
    {
        #region Fields

        private const int Margin = 4;
        private static int DefaultFlaskBarHeight = 122;
        private static int DefaultFlaskBarWigth = 550;
        private static int DefaultExpBarHeight = 24;
        private static int DefaultHeight = 1080;
        private static int DefaultOverlayHeight = 60;

        private Window _view;
        private ClientLurker _Lurker;
        private DockingHelper _dockingHelper;
        private PoeKeyboardHelper _keyboardHelper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBarViewModal"/> class.
        /// </summary>
        /// <param name="lurker">The lurker.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public TradeBarViewModel(ClientLurker lurker, DockingHelper dockingHelper, PoeKeyboardHelper keyboardHelper)
        {
            this._Lurker = lurker;
            this._dockingHelper = dockingHelper;
            this._keyboardHelper = keyboardHelper;
            this.TradeOffers = new ObservableCollection<TradeOfferViewModel>();

            this._dockingHelper.OnWindowMove += this.DockingHelper_OnWindowMove;
            this._Lurker.PoeClosed += this.Lurker_PoeClosed;
            this._Lurker.NewOffer += this.Lurker_NewOffer;
            this._Lurker.TradeAccepted += this.Lurker_TradeAccepted;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the trade offers.
        /// </summary>
        public ObservableCollection<TradeOfferViewModel> TradeOffers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PoeEnded event of the Lurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Lurker_PoeClosed(object sender, EventArgs e)
        {
            this.TryClose();
        }

        /// <summary>
        /// Lurkers the new offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The trade event.</param>
        private void Lurker_NewOffer(object sender, Events.TradeEvent e)
        {
            Execute.OnUIThread(() => this.TradeOffers.Insert(0, new TradeOfferViewModel(e, this._keyboardHelper, this.RemoveOffer)));
        }

        /// <summary>
        /// Lurkers the trade accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_TradeAccepted(object sender, Events.TradeAcceptedEvent e)
        {
            var offer = this.TradeOffers.Where(t => t.Status == OfferStatus.Traded).FirstOrDefault();
            if (offer != null)
            {
                this._keyboardHelper.Kick(offer.PlayerName);
                this.RemoveOffer(offer);
            }
        }

        /// <summary>
        /// Removes the offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void RemoveOffer(TradeOfferViewModel offer)
        {
            if (offer != null)
            {
                Execute.OnUIThread(() => this.TradeOffers.Remove(offer));
            }
        }

        /// <summary>
        /// Handles the OnWindowMove event of the _dockingHelper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DockingHelper_OnWindowMove(object sender, EventArgs e)
        {
            this.SetWindowPosition();
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"></param>
        protected override void OnViewLoaded(object view)
        {
            this._view = view as Window;
            this.SetWindowPosition();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        private void SetWindowPosition()
        {
            Native.GetWindowRect(this._Lurker.PathOfExileProcess.MainWindowHandle, out var poePosition);

            double poeWidth = poePosition.Right - poePosition.Left;
            double poeHeight = poePosition.Bottom - poePosition.Top;

            var expBarHeight = poeHeight * DefaultExpBarHeight / DefaultHeight;
            var flaskBarWidth = poeHeight * DefaultFlaskBarWigth / DefaultHeight;
            var flaskBarHeight = poeHeight * DefaultFlaskBarHeight / DefaultHeight;

            var overlayHeight = DefaultOverlayHeight * flaskBarHeight / DefaultFlaskBarHeight;
            var overlayWidth = (poeWidth - (flaskBarWidth * 2)) / 2;

            Execute.OnUIThread(() =>
            {
                this._view.Height = overlayHeight;
                this._view.Width = overlayWidth;
                this._view.Left = poePosition.Left + flaskBarWidth + Margin;
                this._view.Top = poePosition.Bottom - overlayHeight - expBarHeight - Margin;

            });
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this._dockingHelper.OnWindowMove -= this.DockingHelper_OnWindowMove;
                this._Lurker.PoeClosed -= this.Lurker_PoeClosed;
                this._Lurker.NewOffer -= this.Lurker_NewOffer;
                this._Lurker.TradeAccepted -= this.Lurker_TradeAccepted;
                this._dockingHelper.Dispose();
            }

            base.OnDeactivate(close);
        }

        #endregion
    }
}
