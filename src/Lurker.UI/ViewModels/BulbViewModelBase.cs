//-----------------------------------------------------------------------
// <copyright file="BulbViewModelBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Services;
    using Lurker.UI.Models;
    using PoeLurker.Patreon.Events;

    /// <summary>
    /// BulbViewModel.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public abstract class BulbViewModelBase : PoeOverlayBase
    {
        #region Fields

        /// <summary>
        /// The default bulb height.
        /// </summary>
        protected static readonly int DefaultBulbHeight = 220;

        private INotifyPropertyChanged _actionView;
        private System.Action _stickyAction;
        private INotifyPropertyChanged _stickyActionView;
        private CancellationTokenSource _tokenSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BulbViewModelBase" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The Processs lurker.</param>
        /// <param name="settingsService">The settings serivce.</param>
        /// <param name="clientLurker">The client lurker.</param>
        public BulbViewModelBase(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, ClientLurker clientLurker)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this.ClientLurker = clientLurker;
            this.SetDefaultAction();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has action.
        /// </summary>
        public bool HasAction => this.Action != null;

        /// <summary>
        /// Gets a value indicating whether this instance is default action.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default action; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefaultAction => this.DefaultAction == this.Action;

        /// <summary>
        /// Gets or sets the action view.
        /// </summary>
        public INotifyPropertyChanged ActionView
        {
            get
            {
                return this._actionView;
            }

            protected set
            {
                this._actionView = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the client lurker.
        /// </summary>
        protected ClientLurker ClientLurker { get; private set; }

        /// <summary>
        /// Gets the default action.
        /// </summary>
        protected virtual System.Action DefaultAction => null;

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        protected System.Action Action { get; set; }

        /// <summary>
        /// Gets or sets the sub action.
        /// </summary>
        protected System.Action SubAction { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        public void OnClick()
        {
            if (this.Action == null)
            {
                this.DefaultAction?.Invoke();
                return;
            }

            this.Action.Invoke();
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        protected void Hide()
        {
            this.ActionView = null;
            this.SubAction = null;
            this.Action = this._stickyAction;
            this.ActionView = this._stickyActionView;
            this._stickyAction = null;
            this._stickyActionView = null;
            this.NotifyOfPropertyChange(nameof(this.HasAction));
        }

        /// <summary>
        /// Sets the default action.
        /// </summary>
        protected void SetDefaultAction()
        {
            if (this.DefaultAction == null)
            {
                return;
            }

            EventHandler<LocationChangedEvent> firstLocationChanged = default;
            firstLocationChanged = (s, a) =>
            {
                this.ClientLurker.LocationChanged -= firstLocationChanged;
                this.SetAction(new BulbMessage() { Action = this.DefaultAction });
            };
            this.ClientLurker.LocationChanged += firstLocationChanged;
        }

        /// <summary>
        /// Sets the action.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void SetAction(BulbMessage message)
        {
            if (this._tokenSource != null)
            {
                this._tokenSource.Cancel();
                this._tokenSource.Dispose();
                this._tokenSource = null;
            }

            message.OnShow?.Invoke(this.Hide);

            if (message.Sticky)
            {
                this._stickyAction = this.Action;
                this._stickyActionView = this._actionView;
            }

            var disposableView = this.ActionView as IDisposable;
            if (disposableView != null)
            {
                disposableView.Dispose();
            }

            this.Action = message.Action;
            this.SubAction = message.SubAction;
            this.ActionView = message.View;

            this.NotifyOfPropertyChange(nameof(this.HasAction));

            if (message.DisplayTime != TimeSpan.Zero)
            {
                this._tokenSource = new CancellationTokenSource();
                var token = this._tokenSource.Token;
                Task.Run(async () => await Task.Delay(message.DisplayTime)).ContinueWith((t) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    this.Hide();
                });
            }
        }

        #endregion
    }
}