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
        private System.Action _onClick;

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
            this._onClick?.Invoke();
        }

        /// <summary>
        /// Helps this instance.
        /// </summary>
        /// <param name="onClick">The on click.</param>
        public void Initialize(System.Action onClick)
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

            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyAbsoluteScalingY(value);
                this.View.Width = this.ApplyAbsoluteScalingX(value);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Right - value);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - value);
            });
        }

        #endregion
    }
}