//-----------------------------------------------------------------------
// <copyright file="PoeOverlayBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Represents a Poe Overlay.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    /// <seealso cref="Caliburn.Micro.IViewAware" />
    public abstract class PoeOverlayBase : ScreenBase, IViewAware
    {
        #region Fields

        private bool _manualHide;
        private double _scaleX;
        private double _scaleY;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeOverlayBase" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        public PoeOverlayBase(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService)
            : base(windowManager)
        {
            this.DockingHelper = dockingHelper;
            this.ProcessLurker = processLurker;
            this.SettingsService = settingsService;

            // Default scale value
            this._scaleY = 1;
            this._scaleX = 1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [debug enabled].
        /// </summary>
        public bool DebugEnabled => this.SettingsService.DebugEnabled;

        /// <summary>
        /// Gets the margin.
        /// </summary>
        protected static int Margin => 4;

        /// <summary>
        /// Gets the default height of the flask bar.
        /// </summary>
        protected static int DefaultFlaskBarHeight => 122;

        /// <summary>
        /// Gets the default width of the flask bar.
        /// </summary>
        protected static int DefaultFlaskBarWidth => 550;

        /// <summary>
        /// Gets the default height of the exp bar.
        /// </summary>
        protected static int DefaultExpBarHeight => 24;

        /// <summary>
        /// Gets the default height.
        /// </summary>
        protected static int DefaultHeight => 1080;

        /// <summary>
        /// Gets the view.
        /// </summary>
        protected Window View { get; private set; }

        /// <summary>
        /// Gets the settings service.
        /// </summary>
        protected SettingsService SettingsService { get; private set; }

        /// <summary>
        /// Gets the process lurker.
        /// </summary>
        protected ProcessLurker ProcessLurker { get; private set; }

        /// <summary>
        /// Gets the docking helper.
        /// </summary>
        protected DockingHelper DockingHelper { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the scaling x.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The scaled value.</returns>
        protected double ApplyScalingX(double value)
        {
            return value / this._scaleX;
        }

        /// <summary>
        /// Applies the scalling y.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The scaled value.</returns>
        protected double ApplyScalingY(double value)
        {
            return value / this._scaleY;
        }

        /// <summary>
        /// Hides the view.
        /// </summary>
        protected void HideView()
        {
            this.View?.Hide();
        }

        /// <summary>
        /// Shows the view.
        /// </summary>
        protected void ShowView()
        {
            if (this._manualHide || this.View == null || !this.View.IsLoaded)
            {
                return;
            }

            this.View?.Show();
        }

        /// <summary>
        /// Shows the view.
        /// </summary>
        protected void SetInForeground()
        {
            Execute.OnUIThread(() =>
            {
                var handle = new WindowInteropHelper(this.View).Handle;
                this.DockingHelper.SetForeground(handle);
            });
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
        /// <param name="information">The information.</param>
        private void DockingHelper_OnWindowMove(object sender, PoeWindowInformation information)
        {
            if (this.View != null)
            {
                this.SetWindowPosition(information);
            }
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
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            this.View = view as Window;
            this.View.ShowActivated = false;

            var source = PresentationSource.FromVisual(this.View);
            if (source != null)
            {
                this._scaleX = source.CompositionTarget.TransformToDevice.M11;
                this._scaleY = source.CompositionTarget.TransformToDevice.M22;
            }

            this.SetWindowPosition(this.DockingHelper.WindowInformation);
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected abstract void SetWindowPosition(PoeWindowInformation windowInformation);

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this.ProcessLurker.ProcessClosed += this.Lurker_PoeClosed;
            this.SettingsService.OnSave += this.SettingsService_OnSave;

            if (this.DockingHelper != null)
            {
                this.DockingHelper.OnWindowMove += this.DockingHelper_OnWindowMove;
                this.DockingHelper.OnForegroundChange += this.DockingHelper_OnForegroundChange;
            }

            base.OnActivate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this.ProcessLurker.ProcessClosed -= this.Lurker_PoeClosed;
                this.SettingsService.OnSave -= this.SettingsService_OnSave;
                this.DockingHelper.OnWindowMove -= this.DockingHelper_OnWindowMove;
            }

            base.OnDeactivate(close);
        }

        #endregion
    }
}