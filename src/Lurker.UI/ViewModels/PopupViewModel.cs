//-----------------------------------------------------------------------
// <copyright file="PopupViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Lurker;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using Lurker.UI.Models;
    using Winook;

    /// <summary>
    /// Represent the popup.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    public class PopupViewModel : PoeOverlayBase
    {
        #region Fields

        private const int MouseMargin = 160;
        private const int PopupMargin = 10;
        private MouseLurker _mouseLurker;
        private PoeWindowInformation _windowInformation;
        private int _x;
        private int _y;
        private double _opacity;
        private double _width;
        private double _height;

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
            this._opacity = 1;
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

        /// <summary>
        /// Gets the opacity.
        /// </summary>
        public double Opacity
        {
            get
            {
                return this._opacity;
            }

            private set
            {
                if (this._opacity != value)
                {
                    this._opacity = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void Open(PropertyChangedBase content)
        {
            this.ClearContent();
            this.SetPosition();
            this.SetContent(content);
            this._mouseLurker.MouseMove += this.MouseLurker_MouseMove;
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
        /// Sets the content.
        /// </summary>
        /// <param name="content">The view.</param>
        private void SetContent(PropertyChangedBase content)
        {
            this.PopupContent = content;
            this.NotifyOfPropertyChange(() => this.PopupContent);
            this.NotifyOfPropertyChange(() => this.ContentVisible);
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        private void SetPosition()
        {
            Execute.OnUIThread(() =>
            {
                this._x = this._mouseLurker.X;
                this._y = this._mouseLurker.Y;

                var currentWidth = this.View.Width;
                var rightSide = currentWidth + this._mouseLurker.X;
                if (rightSide > this._windowInformation.Position.Right)
                {
                    this.View.Left = this.ApplyScalingX(this._mouseLurker.X - (rightSide - this._windowInformation.Position.Right) - PopupMargin);
                }
                else
                {
                    this.View.Left = this.ApplyScalingX(this._mouseLurker.X);
                }

                this.View.SizeToContent = System.Windows.SizeToContent.Manual;

                // +10 is to make sure the mouse is not over the overlay
                this.View.Top = this.ApplyScalingY(this._mouseLurker.Y + 10);
                this.View.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            });
        }

        /// <summary>
        /// Clears the content.
        /// </summary>
        private void ClearContent()
        {
            if (this.PopupContent != null && this.PopupContent is IDisposable disposable)
            {
                disposable.Dispose();
            }

            this.PopupContent = null;
            this.NotifyOfPropertyChange(() => this.PopupContent);
            this._mouseLurker.MouseMove -= this.MouseLurker_MouseMove;
        }

        /// <summary>
        /// Handles the MouseMove event of the MouseLurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseMessageEventArgs"/> instance containing the event data.</param>
        private void MouseLurker_MouseMove(object sender, MouseMessageEventArgs e)
        {
            var rayon = this._width / 2;
            var center = new Position()
            {
                X = (int)(this._x + rayon),
                Y = (int)(this._y + (this._height / 2)),
            };

            var differenceX = System.Math.Abs(center.X - e.X);
            var differenceY = System.Math.Abs(center.Y - e.Y);
            var hypothenuse = System.Math.Sqrt(System.Math.Pow(differenceX, 2) + System.Math.Pow(differenceY, 2));

            var difference = MouseMargin + rayon - hypothenuse + 30;
            if (difference <= 0)
            {
                this.ClearContent();
            }
            else
            {
                this.Opacity = difference / MouseMargin;
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the View control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void View_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this._height = e.NewSize.Height;
            this._width = e.NewSize.Width;

            var rightSide = e.NewSize.Width + this._x + 15;
            if (rightSide > this._windowInformation.Position.Right)
            {
                this.View.Left = this.ApplyScalingX(this._x - (rightSide - this._windowInformation.Position.Right) - PopupMargin);
            }
            else
            {
                this.View.Left = this.ApplyScalingX(this._x + 15);
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