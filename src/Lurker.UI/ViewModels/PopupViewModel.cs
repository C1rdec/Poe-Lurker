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

        private const int PopupMargin = 10;
        private MouseLurker _mouseLurker;
        private PoeWindowInformation _windowInformation;
        private int _x;
        private int _y;

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
        /// Sets the position.
        /// </summary>
        public void SetPosition()
        {
            Execute.OnUIThread(() =>
            {
                this._x = this._mouseLurker.X;
                this._y = this._mouseLurker.Y;

                var currentWidth = this.View.Width;
                var rightSide = currentWidth + this._mouseLurker.X;
                if (rightSide > this._windowInformation.Position.Right)
                {
                    this.View.Left = this._mouseLurker.X - (rightSide - this._windowInformation.Position.Right) - PopupMargin;
                }
                else
                {
                    this.View.Left = this._mouseLurker.X;
                }

                this.View.SizeToContent = System.Windows.SizeToContent.Manual;
                this.View.Top = this._mouseLurker.Y;
                this.View.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            });
        }

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
        /// Clears the content.
        /// </summary>
        public void ClearContent()
        {
            this.PopupContent = null;
            this.NotifyOfPropertyChange(() => this.PopupContent);
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.View.SizeChanged += this.View_SizeChanged;
        }

        /// <summary>
        /// Handles the SizeChanged event of the View control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void View_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            var rightSide = e.NewSize.Width + this._x;
            if (rightSide > this._windowInformation.Position.Right)
            {
                this.View.Left = this._x - (rightSide - this._windowInformation.Position.Right) - PopupMargin;
            }
            else
            {
                this.View.Left = this._x;
            }
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            this._windowInformation = windowInformation;
        }

        #endregion
    }
}