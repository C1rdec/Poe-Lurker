namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;

    public class HideoutViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly int DefaultSize = 60;
        PoeKeyboardHelper _keyboardHelper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HideoutViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker"></param>
        /// <param name="settingsService"></param>
        public HideoutViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, PoeKeyboardHelper keyboardHelper) 
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._keyboardHelper = keyboardHelper;
        }

        #endregion

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        public void JoinHideout()
        {
            this._keyboardHelper.JoinHideout();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation"></param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = DefaultSize * windowInformation.Height / 1080;
            var margin = PoeApplicationContext.WindowStyle == WindowStyle.Windowed ? 10 : 0;

            Execute.OnUIThread(() =>
            {
                this._view.Height = value;
                this._view.Width = value;
                this._view.Left = windowInformation.Position.Left + margin;
                this._view.Top = windowInformation.Position.Bottom - value - margin;
            });
        }
    }
}
