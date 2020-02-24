//-----------------------------------------------------------------------
// <copyright file="LifeBulbViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Events;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;
    using System.ComponentModel;

    public class LifeBulbViewModel : PoeOverlayBase, IHandle<LifeBulbMessage>
    {
        #region Fields

        private static readonly int DefaultLifeBulbHeight = 220;
        private INotifyPropertyChanged _actionView;
        private System.Action _action;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeBulbViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="lurker"></param>
        /// <param name="settingsService"></param>
        public LifeBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker lurker, SettingsService settingsService) 
            : base(windowManager, dockingHelper, lurker, settingsService)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has action.
        /// </summary>
        public bool HasAction => this._action != null;

        /// <summary>
        /// Gets the action view.
        /// </summary>
        public INotifyPropertyChanged ActionView
        {
            get
            {
                return this._actionView;
            }

            private set
            {
                this._actionView = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        public void OnClick()
        {
            this._action();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(LifeBulbMessage message)
        {
            message.OnShow(this.Hide);
            this._action = message.Action;
            this.ActionView = message.View;

            this.NotifyOfPropertyChange(nameof(this.HasAction));
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation"></param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultLifeBulbHeight * windowInformation.Height / 1080;
            Execute.OnUIThread(() =>
            {
                this._view.Height = value;
                this._view.Width = value;
                this._view.Left = windowInformation.Position.Left + 6;
                this._view.Top = windowInformation.Position.Bottom - value;
            });
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        private void Hide()
        {
            this.ActionView = null;
            this._action = null;
            this.NotifyOfPropertyChange(nameof(this.HasAction));
        }

        #endregion
    }
}
