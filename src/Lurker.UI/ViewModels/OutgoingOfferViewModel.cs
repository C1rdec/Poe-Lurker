//-----------------------------------------------------------------------
// <copyright file="OutgoingOfferViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Events;
    using Lurker.Helpers;
    using System;

    public class OutgoingOfferViewModel: PropertyChangedBase
    {
        #region Fields

        private static readonly int NumberOfCharacter = 4;
        private OutgoingTradeEvent _event;
        private PoeKeyboardHelper _keyboardHelper;
        private bool _skipMainAction;
        private Action<OutgoingOfferViewModel> _removeCallback;
        private bool _waiting;
        private double _delayToClose;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingOfferViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public OutgoingOfferViewModel(OutgoingTradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper, Action<OutgoingOfferViewModel> removeCallback)
        {
            this._event = tradeEvent;
            this._keyboardHelper = keyboardHelper;
            this._removeCallback = removeCallback;
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
        /// Mains the action.
        /// </summary>
        public void MainAction()
        {
            if (this._skipMainAction)
            {
                this._skipMainAction = false;
                return;
            }

            this._keyboardHelper.JoinHideout(this._event.PlayerName);
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
            this._removeCallback(this);
        }

        #endregion
    }
}
