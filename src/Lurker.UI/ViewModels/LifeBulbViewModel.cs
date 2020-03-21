//-----------------------------------------------------------------------
// <copyright file="LifeBulbViewModel.cs" company="Wohs">
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
    using Lurker.UI.Views;

    public class LifeBulbViewModel : BulbViewModel, IHandle<LifeBulbMessage>
    {
        #region Fields

        private IEventAggregator _eventAggregator;
        private PoeKeyboardHelper _keyboardHelper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeBulbViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="lurker"></param>
        /// <param name="settingsService"></param>H
        public LifeBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker lurker, SettingsService settingsService, PoeKeyboardHelper keyboard) 
            : base(windowManager, dockingHelper, lurker, settingsService)
        {
            this._keyboardHelper = keyboard;
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
        }

        #endregion

        #region Methods

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
        /// <param name="windowInformation"></param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultBulbHeight * windowInformation.Height / 1080;
            Execute.OnUIThread(() =>
            {
                this._view.Height = value;
                this._view.Width = value;
                this._view.Left = windowInformation.Position.Left + 6;
                this._view.Top = windowInformation.Position.Bottom - value;
                var lifeView = this._view as LifeBulbView;
                lifeView.ResizeLifeBulb();
            });
        }

        /// <summary>
        /// Defaults the action.
        /// </summary>
        protected override System.Action DefaultAction => () => this._keyboardHelper.JoinHideout();

        #endregion
    }
}
