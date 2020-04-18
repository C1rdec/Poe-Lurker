//-----------------------------------------------------------------------
// <copyright file="ManaBulbViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
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
    using System;

    public class ManaBulbViewModel : BulbViewModel, IHandle<ManaBulbMessage>
    {
        #region Fields

        private IEventAggregator _eventAggregator;
        private bool _updateRequired;
        private ClientLurker _clientLurker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeBulbViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="lurker"></param>
        /// <param name="settingsService"></param>H
        public ManaBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker clientLurker, ProcessLurker processLurker, SettingsService settingsService) 
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
            this._clientLurker = clientLurker;

            this._clientLurker.LocationChanged += this.Lurker_LocationChanged;
            this._clientLurker.RemainingMonsters += this.Lurker_RemainingMonsters;
            this._settingsService.OnSave += this.SettingsService_OnSave;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defaults the action.
        /// </summary>
        protected override System.Action DefaultAction
        {
            get
            {
                if (!this._settingsService.DashboardEnabled)
                {
                    return null;
                }

                return () => this._eventAggregator.PublishOnUIThread(IoC.Get<DashboardViewModel>());
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
            if (message.NeedToHide)
            {
                this.HideView(8000);
                return;
            }

            if (this._updateRequired && message.IsUpdate)
            {
                base.SetAction(message);
                return;
            }            

            this.SetAction(message);

            if (message.IsUpdate)
            {
                this._updateRequired = true;
            }
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

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this._clientLurker.LocationChanged -= this.Lurker_LocationChanged;
                this._clientLurker.RemainingMonsters -= this.Lurker_RemainingMonsters;
                this._settingsService.OnSave -= this.SettingsService_OnSave;
                this._eventAggregator.Unsubscribe(this);
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Sets the action.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void SetAction(BulbMessage message)
        {
            if (this._updateRequired)
            {
                return;
            }

            base.SetAction(message);
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SettingsService_OnSave(object sender, EventArgs e)
        {
            this.SetDefaultAction();
        }

        /// <summary>
        /// Lurkers the remaining monsters.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_RemainingMonsters(object sender, Patreon.Events.MonstersRemainEvent e)
        {
            this.SetAction(new ManaBulbMessage() { View = new RemainingMonsterViewModel(e), DisplayTime = TimeSpan.FromSeconds(3) });
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
            else
            {
                if (this.IsDefaultAction)
                {
                    this.SetAction(new ManaBulbMessage());
                }
            }
        }

        #endregion
    }
}
