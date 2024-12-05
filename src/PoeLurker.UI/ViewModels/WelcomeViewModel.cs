//-----------------------------------------------------------------------
// <copyright file="WelcomeViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using Caliburn.Micro;
using PoeLurker.Core.Extensions;

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
        ProcessExtensions.OpenUrl("https://discord.com/invite/hQERv7K");
    }

    /// <summary>
    /// Opens the patreon.
    /// </summary>
    public void OpenPatreon()
    {
        ProcessExtensions.OpenUrl("https://www.patreon.com/poelurker");
    }

    /// <summary>
    /// Opens the user guide.
    /// </summary>
    public void OpenUserGuide()
    {
        ProcessExtensions.OpenUrl(@"https://docs.google.com/presentation/d/1XhaSSNAFGxzouc5amzAW8c_6ifToNjnsQq5UmNgLXoo/present?slide=id.p");
    }

    /// <summary>
    /// Opens the cheat sheet.
    /// </summary>
    public void OpenCheatSheet()
    {
        ProcessExtensions.OpenUrl(@"https://github.com/C1rdec/Poe-Lurker/blob/main/assets/CheatSheet.md");
    }

    /// <summary>
    /// Close the window.
    /// </summary>
    public void CloseWindow()
    {
        TryCloseAsync();
    }

    #endregion
}