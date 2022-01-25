//-----------------------------------------------------------------------
// <copyright file="LifeBulbViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using Lurker.UI.Models;
    using Lurker.UI.Views;

    /// <summary>
    /// Represents the life bulb viewmodel.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.BulbViewModelBase" />
    public class LifeBulbViewModel : BulbViewModelBase, IHandle<LifeBulbMessage>
    {
        #region Fields

        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeBulbViewModel" /> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="clientLurker">The client lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        public LifeBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, ClientLurker clientLurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, processLurker, settingsService, clientLurker)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether HasSubAction.
        /// </summary>
        public bool HasSubAction => this.SubAction != null;

        #endregion

        #region Methods

        /// <summary>
        /// Handles the SubAction.
        /// </summary>
        public void OnSubAction()
        {
            this.SubAction?.Invoke();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(LifeBulbMessage message)
        {
            this.SetAction(message);
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultBulbHeight * windowInformation.Height / 1080;
            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyScalingY(value);
                this.View.Width = this.ApplyScalingX(value);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left + 10);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - value - 10);
                var lifeView = this.View as LifeBulbView;
                lifeView.ResizeLifeBulb();
            });
        }

        #endregion
    }
}