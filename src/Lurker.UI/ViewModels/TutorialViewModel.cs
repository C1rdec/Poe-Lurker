﻿//-----------------------------------------------------------------------
// <copyright file="TutorialViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;

    /// <summary>
    /// Represents the tutorial.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    public class TutorialViewModel : ScreenBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TutorialViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        public TutorialViewModel(IWindowManager windowManager)
            : base(windowManager)
        {
        }

        #endregion
    }
}