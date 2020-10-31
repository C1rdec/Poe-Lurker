//-----------------------------------------------------------------------
// <copyright file="ScreenBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;

    /// <summary>
    /// Represents the screen base class.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.Screen" />
    public class ScreenBase : Screen
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
            Execute.OnUIThread(() =>
            {
                this._windowManager.ShowWindow(this);
                base.OnActivate();
            });
        }

        #endregion
    }
}