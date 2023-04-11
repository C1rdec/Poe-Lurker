//-----------------------------------------------------------------------
// <copyright file="ManaBulbViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the Manabulbviewmodel.
    /// </summary>
    public class ManaBulbViewModel : BulbViewModelBase, IHandle<ManaBulbMessage>
    {
        #region Fields

        private IEventAggregator _eventAggregator;
        private bool _updateRequired;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManaBulbViewModel" /> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="clientLurker">The client lurker.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// H
        public ManaBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker clientLurker, ProcessLurker processLurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, processLurker, settingsService, clientLurker)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);

            this.ClientLurker.LocationChanged += this.Lurker_LocationChanged;
            this.SettingsService.OnSave += this.SettingsService_OnSave;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default action.
        /// </summary>
        protected override System.Action DefaultAction
        {
            get
            {
                if (!this.SettingsService.DashboardEnabled)
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
        /// <param name="windowInformation">The wndow information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultBulbHeight * windowInformation.Height / 1080;
            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyAbsoluteScalingY(value);
                this.View.Width = this.ApplyAbsoluteScalingX(value);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Right - value);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - value);
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
                this.ClientLurker.LocationChanged -= this.Lurker_LocationChanged;
                this.SettingsService.OnSave -= this.SettingsService_OnSave;
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
                        Action = this.DefaultAction,
                        View = new LeagueViewModel(this._eventAggregator),
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