//-----------------------------------------------------------------------
// <copyright file="TradeBarViewModel.cs" company="Wohs">
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
    using NAudio.Wave;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    public class TradebarViewModel : ScreenBase, IViewAware
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
        private TradebarContext _context;
        private List<OfferViewModel> _activeOffers = new List<OfferViewModel>();
        private SettingsService _settingsService;
        private double _itemNameVerticalOffset;
        private double _itemNameHorizontalOffset;
        private double _itemNameHeight;
        private double _itemNameWidth;
        private string _itemName;
        private bool _hasActiveOffer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBarViewModal"/> class.
        /// </summary>
        /// <param name="lurker">The lurker.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public TradebarViewModel(ClientLurker lurker, DockingHelper dockingHelper, PoeKeyboardHelper keyboardHelper, SettingsService settingsService, IWindowManager windowManager)
            : base (windowManager)
        {
            this._Lurker = lurker;
            this._dockingHelper = dockingHelper;
            this._keyboardHelper = keyboardHelper;
            this._settingsService = settingsService;
            this._settingsService.OnSave += this.SettingsService_OnSave;
            this.TradeOffers = new ObservableCollection<OfferViewModel>();

            this._dockingHelper.OnWindowMove += this.DockingHelper_OnWindowMove;

            this._Lurker.PoeClosed += this.Lurker_PoeClosed;
            this._Lurker.NewOffer += this.Lurker_NewOffer;
            this._Lurker.TradeAccepted += this.Lurker_TradeAccepted;
            this._Lurker.PlayerJoined += this.Lurker_PlayerJoined;
            this._Lurker.PlayerLeft += this.Lurker_PlayerLeft;
            this.PropertyChanged += this.TradebarViewModel_PropertyChanged;

            this._context = new TradebarContext(this.RemoveOffer, this.AddActiveOffer, this.SetActiveOffer);
            this.DisplayName = "Poe Lurker";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the trade offers.
        /// </summary>
        public ObservableCollection<OfferViewModel> TradeOffers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has active offer.
        /// </summary>
        public bool HasActiveOffer
        {
            get
            {
                return this._hasActiveOffer;
            }

            set
            {
                this._hasActiveOffer = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the item name vertical offset.
        /// </summary>
        public double ItemNameVerticalOffset
        {
            get
            {
                return this._itemNameVerticalOffset;
            }

            set
            {
                this._itemNameVerticalOffset = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the item name horizontal offset.
        /// </summary>
        public double ItemNameHorizontalOffset
        {
            get
            {
                return this._itemNameHorizontalOffset;
            }

            set
            {
                this._itemNameHorizontalOffset = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the height of the item name.
        /// </summary>
        public double ItemNameHeight
        {
            get
            {
                return this._itemNameHeight;
            }

            set
            {
                this._itemNameHeight = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the width of the item name.
        /// </summary>
        public double ItemNameWidth
        {
            get
            {
                return this._itemNameWidth;
            }

            set
            {
                this._itemNameWidth = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string ItemName
        {
            get
            {
                return this._itemName;
            }

            set
            {
                this._itemName = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [debug enabled].
        /// </summary>
        public bool DebugEnabled => this._settingsService.DebugEnabled;

        /// <summary>
        /// Gets the active offer.
        /// </summary>
        private OfferViewModel ActiveOffer => this._activeOffers.FirstOrDefault();

        #endregion

        #region Methods

        /// <summary>
        /// Searches the item.
        /// </summary>
        public void SearchItem()
        {
            var activeOffer = this.ActiveOffer;
            if (activeOffer != null)
            {
                this._keyboardHelper.Search(activeOffer.BuildSearchItemName());
            }
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SettingsService_OnSave(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.DebugEnabled));
        }

        /// <summary>
        /// Handles the PropertyChanged event of the TradebarViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TradebarViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ItemName))
            {
                this.HasActiveOffer = string.IsNullOrEmpty(this.ItemName) ? false : true;
            }
        }

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
            if (this._settingsService.AlertEnabled)
            {
                this.PlayAlert();
            }

            Execute.OnUIThread(() => 
            {
                this.TradeOffers.Add(new OfferViewModel(e, this._keyboardHelper, this._context, this._settingsService));
            });
        }

        /// <summary>
        /// Plays the alert.
        /// </summary>
        private void PlayAlert()
        {
            var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Lurker.UI.Assets.TradeAlert.mp3");
            var waveOut = new WaveOutEvent();
            var mp3Reader = new Mp3FileReader(stream);
            waveOut.Init(mp3Reader);
            waveOut.Volume = this._settingsService.AlertVolume;
            waveOut.Play();

            EventHandler<StoppedEventArgs> handler = default;
            handler = (object s, StoppedEventArgs e) =>
            {
                stream.Dispose();
                mp3Reader.Dispose();
                waveOut.Dispose();
                waveOut.PlaybackStopped -= handler;
            };

            waveOut.PlaybackStopped += handler;
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
                if (!string.IsNullOrEmpty(this._settingsService.ThankYouMessage))
                {
                    this._keyboardHelper.Whisper(offer.PlayerName, this._settingsService.ThankYouMessage);
                }

                this._keyboardHelper.Kick(offer.PlayerName);
                this.RemoveOffer(offer);
            }
        }

        /// <summary>
        /// Lurkers the player joined.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The PLayerJoined Event.</param>
        private void Lurker_PlayerJoined(object sender, Events.PlayerJoinedEvent e)
        {
            foreach (var offer in this.TradeOffers.Where(o => o.PlayerName == e.PlayerName))
            {
                offer.BuyerInSameInstance = true;
            }
        }

        /// <summary>
        /// Lurkers the player left.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_PlayerLeft(object sender, Events.PlayerLeftEvent e)
        {
            foreach (var offer in this.TradeOffers.Where(o => o.PlayerName == e.PlayerName))
            {
                offer.BuyerInSameInstance = false;
            }
        }

        /// <summary>
        /// Removes the offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void RemoveOffer(OfferViewModel offer)
        {
            if (offer != null)
            {
                Execute.OnUIThread(() => 
                { 
                    this.TradeOffers.Remove(offer);
                    this._activeOffers.Remove(offer);
                    this.ItemName = string.Empty;

                    var activeOffer = this.ActiveOffer;
                    if (this.ActiveOffer != null)
                    {
                        this.ItemName = this.ActiveOffer.ItemName;
                        this.ActiveOffer.Active = true;
                    }

                    offer.Dispose();
                });
            }
        }

        /// <summary>
        /// Adds the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void AddActiveOffer(OfferViewModel offer)
        {
            this._activeOffers.Add(offer);
            this.ItemName = this.ActiveOffer.ItemName;
            this.ActiveOffer.Active = true;
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void SetActiveOffer(OfferViewModel offer)
        {
            var currentActiveOffer = this.ActiveOffer;
            if (currentActiveOffer != null)
            {
                currentActiveOffer.Active = false;
            }

            var index = this._activeOffers.IndexOf(offer);
            if (index != -1)
            {
                this._activeOffers.RemoveAt(index);
            }

            // Dont add if it's the same offer
            if (currentActiveOffer == offer)
            {
                this.ItemName = this.ActiveOffer?.ItemName;
                if (this.ActiveOffer != null)
                {
                    this.ActiveOffer.Active = true;
                }

                return;
            }

            this._activeOffers.Insert(0, offer);
            this.ItemName = offer.ItemName;
            this.ActiveOffer.Active = true;
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
                this._settingsService.OnSave -= this.SettingsService_OnSave;
                this._dockingHelper.Dispose();
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Sorts the offer.
        /// </summary>
        private void SortOffer()
        {
            var collection = this.TradeOffers;
            var sortableList = new List<OfferViewModel>(collection);
            var offers = sortableList.OrderByDescending(t => t.Waiting);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(offers.ElementAt(i)), i);
            }
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

            this.ItemNameWidth = (flaskBarWidth * 0.24);
            this.ItemNameHeight = expBarHeight;

            this.ItemNameVerticalOffset = (flaskBarHeight * 0.30 * -1) - this.ItemNameHeight;

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
