//-----------------------------------------------------------------------
// <copyright file="WelcomeViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Diagnostics;
    using Caliburn.Micro;

    /// <summary>
    /// Represents the welcome screen.
    /// </summary>
    public class WelcomeViewModel : Screen
    {
        #region Methods

        /// <summary>
        /// Opens the discord.
        /// </summary>
        public void OpenDiscord()
        {
            Process.Start("https://discord.com/invite/hQERv7K");
        }

        /// <summary>
        /// Opens the patreon.
        /// </summary>
        public void OpenPatreon()
        {
            Process.Start("https://www.patreon.com/poelurker");
        }

        /// <summary>
        /// Opens the user guide.
        /// </summary>
        public void OpenUserGuide()
        {
            Process.Start(@"https://docs.google.com/presentation/d/1XhaSSNAFGxzouc5amzAW8c_6ifToNjnsQq5UmNgLXoo/present?slide=id.p");
        }

        /// <summary>
        /// Opens the cheat sheet.
        /// </summary>
        public void OpenCheatSheet()
        {
            Process.Start(@"https://github.com/C1rdec/Poe-Lurker/blob/master/assets/CheatSheet.md");
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        public void CloseWindow()
        {
            this.TryClose();
        }

        #endregion
    }
}