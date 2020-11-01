//-----------------------------------------------------------------------
// <copyright file="PopupViewModel.cs" company="Wohs Inc.">
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

    /// <summary>
    /// Represent the popup.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    public class PopupViewModel : PoeOverlayBase
    {
        #region Fields

        private MouseLurker _mouseLurker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="mouseLurker">The mouse lurker.</param>
        public PopupViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, MouseLurker mouseLurker)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._mouseLurker = mouseLurker;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content.
        /// </summary>
        public PropertyChangedBase PopupContent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [content visible].
        /// </summary>
        public bool ContentVisible => this.PopupContent != null;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the content.
        /// </summary>
        /// <param name="content">The view.</param>
        public void SetContent(PropertyChangedBase content)
        {
            this.PopupContent = content;
            this.NotifyOfPropertyChange(() => this.PopupContent);
            this.NotifyOfPropertyChange(() => this.ContentVisible);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            // Base first to set the View
            base.OnActivate();

            Execute.OnUIThread(() =>
            {
                this.View.SizeToContent = System.Windows.SizeToContent.Manual;
                this.View.Top = this._mouseLurker.Y;
                this.View.Left = this._mouseLurker.X;
                this.View.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            });

            if (this.View != null)
            {
                this.View.Deactivated += this.View_Deactivated;
            }
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.View.Activate();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
        }

        /// <summary>
        /// Handles the Deactivated event of the View control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void View_Deactivated(object sender, System.EventArgs e)
        {
            if (this.PopupContent != null)
            {
                this.SetContent(null);
            }
        }

        #endregion
    }
}