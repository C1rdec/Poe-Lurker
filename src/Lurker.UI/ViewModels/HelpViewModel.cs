//-----------------------------------------------------------------------
// <copyright file="HelpViewModel.cs" company="Wohs Inc.">
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
    /// Represents the HelpViewModel.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class HelpViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly int DefaultSize = 60;
        private IWindowManager _windowManager;
        private System.Action<bool> _onClick;
        private bool _isShown;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        public HelpViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._windowManager = windowManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Helps this instance.
        /// </summary>
        public void Help()
        {
            this._isShown = !this._isShown;
            this._onClick?.Invoke(this._isShown);
        }

        /// <summary>
        /// Helps this instance.
        /// </summary>
        /// <param name="onClick">The on click.</param>
        public void Initialize(System.Action<bool> onClick)
        {
            this._onClick = onClick;
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultSize * windowInformation.Height / 1080;
            var margin = PoeApplicationContext.WindowStyle == WindowStyle.Windowed ? 10 : 0;

            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyScalingY(value);
                this.View.Width = this.ApplyScalingX(value);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Right - value - margin);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - value - margin);
            });
        }

        #endregion
    }
}