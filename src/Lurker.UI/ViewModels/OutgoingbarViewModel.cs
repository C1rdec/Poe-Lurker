//-----------------------------------------------------------------------
// <copyright file="OutgoingbarViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Timers;

    /// <summary>
    /// Represents the outgoing bar view model.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    /// <seealso cref="Caliburn.Micro.IViewAware" />
    public class OutgoingbarViewModel : PoeOverlayBase
    {
        #region Fields

        protected static int DefaultWidth = 55;
        private ClientLurker _lurker;
        private PoeKeyboardHelper _keyboardHelper;
        private Timer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingbarViewModel"/> class.
        /// </summary>
        /// <param name="lurker">The lurker.</param>
        /// <param name="windowManager">The window manager.</param>
        public OutgoingbarViewModel(ClientLurker lurker, DockingHelper dockingHelper, PoeKeyboardHelper keyboardHelper, SettingsService settingsService, IWindowManager windowManager) 
            : base(windowManager, dockingHelper, lurker, settingsService)
        {
            this._timer = new Timer(50);
            this._timer.Elapsed += this.Timer_Elapsed;
            this.Offers = new ObservableCollection<OutgoingOfferViewModel>();
            this._keyboardHelper = keyboardHelper;
            this._lurker = lurker;

            this._lurker.OutgoingOffer += this.Lurker_OutgoingOffer;
            this.Offers.CollectionChanged += this.Offers_CollectionChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the offers.
        /// </summary>
        public ObservableCollection<OutgoingOfferViewModel> Offers { get; set; }

        /// <summary>
        /// Gets a value indicating whether [any offer].
        /// </summary>
        public bool AnyOffer => this.Offers.Any();

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Elapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {

                foreach (var offer in this.Offers.Where(o => !o.Waiting))
                {
                    offer.DelayToClose = offer.DelayToClose - 0.15;

                    if (offer.DelayToClose <= 0)
                    {
                        return;
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                // An offer has been deleted (Will need to handle this scenario)
                return;
            }
        }

        /// <summary>
        /// Lurkers the outgoing offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_OutgoingOffer(object sender, Events.OutgoingTradeEvent e)
        {
            if (this.Offers.Any(o => o.Event.Equals(e)))
            {
                return;
            }

            Execute.OnUIThread(() => this.Offers.Insert(0, new OutgoingOfferViewModel(e, this._keyboardHelper, this.RemoveOffer)));
        }

        /// <summary>
        /// Removes the offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void RemoveOffer(OutgoingOfferViewModel offer)
        {
            this._timer.Stop();
            Execute.OnUIThread(() => this.Offers.Remove(offer));

            this._timer.Start();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation"></param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var yPosition = windowInformation.FlaskBarWidth * (238 / (double)DefaultFlaskBarWidth);
            var width = windowInformation.Height * DefaultWidth / 1080;
            Execute.OnUIThread(() =>
            {
                this._view.Height = windowInformation.FlaskBarHeight - (Margin * 2);
                this._view.Width = width;
                this._view.Left = windowInformation.Position.Left + yPosition;
                this._view.Top = windowInformation.Position.Bottom - windowInformation.FlaskBarHeight + Margin;
            });
        }
        /// <summary>
        /// Handles the CollectionChanged event of the Offers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Offers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Offers.Any(o => !o.Waiting))
            {
                this._timer.Start();
            }
            else
            {
                this._timer.Stop();
            }

            this.NotifyOfPropertyChange("AnyOffer");
        }

        #endregion
    }
}
