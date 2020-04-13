//-----------------------------------------------------------------------
// <copyright file="PoeOverlayBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public abstract class PoeOverlayBase : ScreenBase, IViewAware
    {
        #region Fields

        protected static int Margin = 4;
        protected static int DefaultFlaskBarHeight = 122;
        protected static int DefaultFlaskBarWidth = 550;
        protected static int DefaultExpBarHeight = 24;
        protected static int DefaultHeight = 1080;

        protected Window _view;
        protected SettingsService _settingsService;
        protected ClientLurker _lurker;
        private DockingHelper _dockingHelper;
        private bool _manualHide;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeOverlayBase"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        public PoeOverlayBase(IWindowManager windowManager, DockingHelper dockingHelper, ClientLurker lurker, SettingsService settingsService) 
            : base(windowManager)
        {
            this._dockingHelper = dockingHelper;
            this._lurker = lurker;
            this._settingsService = settingsService;

            this._dockingHelper.OnWindowMove += this.DockingHelper_OnWindowMove;
            this._dockingHelper.OnForegroundChange += this.DockingHelper_OnForegroundChange;
            this._lurker.PoeClosed += this.Lurker_PoeClosed;
            this._settingsService.OnSave += this.SettingsService_OnSave;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [debug enabled].
        /// </summary>
        public bool DebugEnabled => this._settingsService.DebugEnabled;

        #endregion

        #region Methods

        /// <summary>
        /// Hides the view.
        /// </summary>
        protected void HideView()
        {
            this._view.Hide();
        }

        /// <summary>
        /// Shows the view.
        /// </summary>
        protected void ShowView()
        {
            if (this._manualHide)
            {
                return;
            }

            this._view.Show();
        }

        /// <summary>
        /// Hides the view.
        /// </summary>
        /// <param name="time">The time.</param>
        protected async void HideView(int time)
        {
            this.HideView();
            this._manualHide = true;
            await Task.Delay(time);
            this._manualHide = false;
            this.ShowView();
        }

        /// <summary>
        /// Dockings the helper on foreground change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">if set to <c>true</c> [e].</param>
        private void DockingHelper_OnForegroundChange(object sender, bool e)
        {
            if (e)
            {
                this.ShowView();
            }
            else
            {
                this.HideView();
            }
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SettingsService_OnSave(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.DebugEnabled));
        }

        /// <summary>
        /// Handles the OnWindowMove event of the DockingHelper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DockingHelper_OnWindowMove(object sender, PoeWindowInformation position)
        {
            this.SetWindowPosition(position);
        }

        /// <summary>
        /// Handles the PoeEnded event of the Lurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Lurker_PoeClosed(object sender, EventArgs e)
        {
            this.TryClose();
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"></param>
        protected override void OnViewLoaded(object view)
        {
            this._view = view as Window;
            this.SetWindowPosition(this._dockingHelper.WindowInformation);
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        protected abstract void SetWindowPosition(PoeWindowInformation windowInformation);

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this._lurker.PoeClosed -= this.Lurker_PoeClosed;
                this._settingsService.OnSave -= this.SettingsService_OnSave;
                this._dockingHelper.OnWindowMove -= this.DockingHelper_OnWindowMove;
            }

            base.OnDeactivate(close);
        }

        #endregion
    }
}
