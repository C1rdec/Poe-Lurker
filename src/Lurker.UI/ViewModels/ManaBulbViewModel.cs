//-----------------------------------------------------------------------
// <copyright file="ManaBulbViewModel.cs" company="Wohs">
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
    using System;

    public class ManaBulbViewModel : BulbViewModel, IHandle<ManaBulbMessage>
    {
        #region Fields

        private IEventAggregator _eventAggregator;
        private SettingsViewModel _settingsViewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeBulbViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="lurker"></param>
        /// <param name="settingsService"></param>H
        public ManaBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker lurker, SettingsService settingsService, PoeKeyboardHelper keyboard, SettingsViewModel settingsViewModel) 
            : base(windowManager, dockingHelper, lurker, settingsService)
        {
            this._settingsViewModel = settingsViewModel;
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);

            lurker.LocationChanged += this.Lurker_LocationChanged;
            lurker.RemainingMonsters += this.Lurker_RemainingMonsters;
        }

        /// <summary>
        /// Lurkers the remaining monsters.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_RemainingMonsters(object sender, Patreon.Events.MonstersRemainEvent e)
        {
            this.SetAction(new ManaBulbMessage() { View = new RemainingMonsterViewModel(e), DisplayTime = TimeSpan.FromSeconds(3)});
        }

        /// <summary>
        /// Lurkers the location changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_LocationChanged(object sender, Patreon.Events.LocationChangedEvent e)
        {
            if (e.Location.EndsWith("Hideout"))
            {
                if (!this.HasAction)
                {
                    var message = new ManaBulbMessage()
                    {
                        Action = this.DefaultAction
                    };
                    this.SetAction(message);
                }
            }

        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ManaBulbMessage message)
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
                this._view.Left = windowInformation.Position.Right - value -  6;
                this._view.Top = windowInformation.Position.Bottom - value;
                var lifeView = this._view as ManaBulbView;
            });
        }

        #endregion
    }
}
