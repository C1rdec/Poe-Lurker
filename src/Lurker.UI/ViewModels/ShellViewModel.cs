//-----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using Caliburn.Micro;
    using Lurker.UI.Helpers;
    using Lurker.UI.ViewModels;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    public class ShellViewModel : Screen, IShell, IViewAware
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            this.TradeOffers = new ObservableCollection<TradeOfferViewModel>();
            this._Lurker = new ClientLurker();
            this._dockingHelper = new DockingHelper(this._Lurker.PathOfExileProcess);

            this._dockingHelper.OnWindowMove += this._dockingHelper_OnWindowMove;
            this._Lurker.NewOffer += this.Lurker_NewOffer;
        }

        private void _dockingHelper_OnWindowMove(object sender, EventArgs e)
        {
            this.SetWindowPosition();
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
        /// Lurkers the new offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The trade event.</param>
        private void Lurker_NewOffer(object sender, Events.TradeEvent e)
        {
            this.TradeOffers.Insert(0, new TradeOfferViewModel(e, this.DeleteTradeOffer));
        }

        /// <summary>
        /// Deletes the trade offer.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        private void DeleteTradeOffer(TradeOfferViewModel viewModel)
        {
            this.TradeOffers.Remove(viewModel);
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

        #endregion
    }
}