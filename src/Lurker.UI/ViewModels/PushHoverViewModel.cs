//-----------------------------------------------------------------------
// <copyright file="PushHoverViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Diagnostics;
    using Lurker.Patreon.Services;

    /// <summary>
    /// The push bullet view model.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class PushHoverViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private PushHoverService _service;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PushHoverViewModel" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public PushHoverViewModel(PushHoverService service)
        {
            this._service = service;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public string UserId
        {
            get
            {
                return this._service.UserId;
            }

            set
            {
                this._service.UserId = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        public string Token
        {
            get
            {
                return this._service.Token;
            }

            set
            {
                this._service.Token = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable].
        /// </summary>
        public bool Enable
        {
            get
            {
                return this._service.Enable;
            }

            set
            {
                this._service.Enable = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the thresold.
        /// </summary>
        public int Threshold
        {
            get
            {
                return this._service.Threshold;
            }

            set
            {
                this._service.Threshold = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send a Test.
        /// </summary>
        public async void Test()
        {
            await this._service.SendTestAsync();
        }

        /// <summary>
        /// New Account.
        /// </summary>
        public void NewAccount()
        {
            Process.Start("https://pushover.net/signup");
        }

        /// <summary>
        /// Open the info.md.
        /// </summary>
        public void Info()
        {
            Process.Start("https://github.com/C1rdec/Poe-Lurker/blob/master/assets/Pushover.md");
        }

        #endregion
    }
}