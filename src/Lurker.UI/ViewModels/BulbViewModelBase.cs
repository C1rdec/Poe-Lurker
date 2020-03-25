//-----------------------------------------------------------------------
// <copyright file="BulbViewModelBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class BulbViewModel : PoeOverlayBase
    {
        #region Fields

        private INotifyPropertyChanged _actionView;
        protected static readonly int DefaultBulbHeight = 220;
        protected System.Action _action;
        private System.Action _previousAction;
        private INotifyPropertyChanged _previousActionView;
        private CancellationTokenSource _tokenSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeBulbViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="lurker"></param>
        /// <param name="settingsService"></param>H
        public BulbViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker lurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, lurker, settingsService)
        {
            this.SetDefaultAction();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has action.
        /// </summary>
        public bool HasAction => this._action != null;

        /// <summary>
        /// Gets a value indicating whether this instance is default action.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default action; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefaultAction => this.DefaultAction == this._action;

        /// <summary>
        /// Gets the action view.
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
        /// Defaults the action.
        /// </summary>
        protected virtual System.Action DefaultAction => null;

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        public void OnClick()
        {
            if (this._action == null)
            {
                this.DefaultAction?.Invoke();
                return;
            }

            this._action.Invoke();
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        protected void Hide()
        {
            this.ActionView = null;
            this._action = this._previousAction;
            this.ActionView = this._previousActionView;
            this._previousAction = null;
            this._previousActionView = null;
            this.NotifyOfPropertyChange(nameof(this.HasAction));
        }

        /// <summary>
        /// Sets the default action.
        /// </summary>
        protected void SetDefaultAction()
        {
            this.SetAction(new BulbMessage() { Action = this.DefaultAction });
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

            if (this._previousAction == null)
            {
                this._previousAction = this._action;
                this._previousActionView = this._actionView;
            }

            this._action = message.Action;
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
