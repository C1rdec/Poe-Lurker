//-----------------------------------------------------------------------
// <copyright file="ScreenBase.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;

    public class ScreenBase: Screen
    {
        #region Fields

        private IWindowManager _windowManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenBase"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        public ScreenBase(IWindowManager windowManager)
        {
            this._windowManager = windowManager;
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this._windowManager.ShowWindow(this);
            base.OnActivate();
        }

        #endregion
    }
}
