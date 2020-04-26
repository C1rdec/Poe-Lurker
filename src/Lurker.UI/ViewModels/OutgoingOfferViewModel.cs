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
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the outgoing offer.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class OutgoingOfferViewModel : PropertyChangedBase
    {
        #region Fields

        private static readonly int NumberOfCharacter = 4;
        private OutgoingTradeEvent _event;
        private PoeKeyboardHelper _keyboardHelper;
        private bool _skipMainAction;
        private bool _waiting;
        private bool _active;
        private double _delayToClose;
        private OutgoingbarContext _barContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingOfferViewModel" /> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        /// <param name="context">The context.</param>
        public OutgoingOfferViewModel(OutgoingTradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper, OutgoingbarContext context)
        {
            this._event = tradeEvent;
            this._keyboardHelper = keyboardHelper;
            this._barContext = context;
            this.DelayToClose = 100;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (this._event.PlayerName.Length <= NumberOfCharacter)
                {
                    return this._event.PlayerName;
                }

                return this._event.PlayerName.Substring(0, NumberOfCharacter);
            }
        }

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
                    this.Remove();
                }

                this._delayToClose = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Whoes the is.
        /// </summary>
        public void WhoIs()
        {
            this._keyboardHelper.WhoIs(this._event.PlayerName);
        }

        /// <summary>
        /// Mains the action.
        /// </summary>
        public void MainAction()
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

            this._keyboardHelper.JoinHideout(this._event.PlayerName);
            this._barContext.SetActiveOffer(this);
        }

        /// <summary>
        /// Res the send the offer.
        /// </summary>
        public void ReSend()
        {
            this._skipMainAction = true;
            this._keyboardHelper.Whisper(this._event.PlayerName, this._event.WhisperMessage);
            this.Waiting = true;
            this.DelayToClose = 100;
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this._skipMainAction = true;
            this._barContext.RemoveOffer(this);
        }

        /// <summary>
        /// Sets the active.
        /// </summary>
        public void SetActive()
        {
            this.DelayToClose = 100;
            this.Active = true;
        }

        #endregion
    }
}