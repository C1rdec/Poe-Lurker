//-----------------------------------------------------------------------
// <copyright file="HideoutViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the Hideout overlay.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class HideoutViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly int DefaultSize = 60;
        private PoeKeyboardHelper _keyboardHelper;
        private bool _isVisible;
        private bool _guildVisible;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HideoutViewModel" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        /// <param name="clientLurker">The client lurker.</param>
        public HideoutViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, PoeKeyboardHelper keyboardHelper, ClientLurker clientLurker)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._keyboardHelper = keyboardHelper;
            this.IsVisible = true;
            this.GuildVisible = this.SettingsService.GuildHideoutEnabled;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }

            private set
            {
                this._isVisible = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        public bool GuildVisible
        {
            get
            {
                return this._guildVisible;
            }

            private set
            {
                this._guildVisible = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Joins the Guild hideout.
        /// </summary>
        public async void JoinGuildHideout()
        {
            await this._keyboardHelper.JoinGuildHideout();
        }

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        public async void JoinHideout()
        {
            await this._keyboardHelper.JoinHideout();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultSize * windowInformation.Height / 1080;

            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyAbsoluteScalingY(value);
                this.View.Width = this.ApplyAbsoluteScalingX(value);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - value);
            });
        }

        /// <summary>
        /// On activate.
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            this.SettingsService.OnSave += this.SettingsService_OnSave;
        }

        /// <summary>
        /// On Deactivate.
        /// </summary>
        /// <param name="close">If closing.</param>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            this.SettingsService.OnSave -= this.SettingsService_OnSave;
        }

        private void SettingsService_OnSave(object sender, System.EventArgs e)
        {
            this.GuildVisible = this.SettingsService.GuildHideoutEnabled;
        }

        #endregion
    }
}