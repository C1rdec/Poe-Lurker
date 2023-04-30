//-----------------------------------------------------------------------
// <copyright file="OutgoingOfferViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Patreon.Events;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the outgoing offer.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class OutgoingOfferViewModel : PropertyChangedBase
    {
        #region Fields

        private OutgoingTradeEvent _event;
        private PoeKeyboardHelper _keyboardHelper;
        private bool _skipMainAction;
        private bool _waiting;
        private bool _active;
        private double _delayToClose;
        private OutgoingbarContext _barContext;
        private DockingHelper _dockingHelper;
        private SettingsService _settingsService;
        private double _times;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingOfferViewModel" /> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        /// <param name="context">The context.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="settingsService">The settings service.</param>
        public OutgoingOfferViewModel(OutgoingTradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper, OutgoingbarContext context, DockingHelper dockingHelper, SettingsService settingsService)
        {
            this._event = tradeEvent;
            this._keyboardHelper = keyboardHelper;
            this._barContext = context;
            this._dockingHelper = dockingHelper;
            this._settingsService = settingsService;
            this._times = settingsService.OutgoingDelayToClose;
            this.DelayToClose = 100;
            this.PriceValue = tradeEvent.Price.CalculateValue();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the price value.
        /// </summary>
        public double PriceValue { get; }

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string PlayerName => this._event.PlayerName;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OutgoingOfferViewModel"/> is waiting.
        /// </summary>
        public bool Waiting
        {
            get
            {
                return this._waiting;
            }

            set
            {
                this._waiting = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OutgoingOfferViewModel"/> is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return this._active;
            }

            set
            {
                this._active = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public TradeEvent Event => this._event;

        /// <summary>
        /// Gets or sets the delay to close.
        /// </summary>
        public double DelayToClose
        {
            get
            {
                return this._delayToClose;
            }

            set
            {
                if (value <= 0)
                {
                    this.RemoveCore(false);
                }

                this._delayToClose = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the trade value.
        /// </summary>
        public TradeValueViewModel TradeValue => new TradeValueViewModel(this._event, this._settingsService);

        #endregion

        #region Methods

        /// <summary>
        /// Tick the delay to close.
        /// </summary>
        public void Tick()
        {
            this._times -= 0.15;
            this.DelayToClose = this._times * 100 / this._settingsService.OutgoingDelayToClose;
        }

        /// <summary>
        /// Whoes the is.
        /// </summary>
        public async void WhoIs()
        {
            await this._keyboardHelper.WhoIs(this._event.PlayerName);
        }

        /// <summary>
        /// Mains the action.
        /// </summary>
        public async void MainAction()
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                this._barContext.SetActiveOffer(this);
                return;
            }

            if (this._skipMainAction)
            {
                this._skipMainAction = false;
                return;
            }

            await this._keyboardHelper.JoinHideout(this._event.PlayerName);
            this._barContext.SetActiveOffer(this);
        }

        /// <summary>
        /// Res the send the offer.
        /// </summary>
        public async void ReSend()
        {
            this._skipMainAction = true;
            await this._keyboardHelper.Whisper(this._event.PlayerName, this._event.WhisperMessage);
            this.Waiting = true;
            this.DelayToClose = 100;
            this._times = this._settingsService.OutgoingDelayToClose;
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this.RemoveCore(true);
        }

        /// <summary>
        /// Removes the core.
        /// </summary>
        /// <param name="setForeground">if set to <c>true</c> [set foreground].</param>
        public void RemoveCore(bool setForeground)
        {
            this._skipMainAction = true;
            var controlPressed = false;
            Execute.OnUIThread(() => controlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            if (controlPressed)
            {
                this._barContext.ClearAll();
            }
            else
            {
                this._barContext.RemoveOffer(this);
            }

            if (setForeground)
            {
                this._dockingHelper.SetForeground();
            }
        }

        /// <summary>
        /// Sets the active.
        /// </summary>
        public void SetActive()
        {
            this.DelayToClose = 100;
            this._times = this._settingsService.OutgoingDelayToClose;
            this.Active = true;
        }

        #endregion
    }
}