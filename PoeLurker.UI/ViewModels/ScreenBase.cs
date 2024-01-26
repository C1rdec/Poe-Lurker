//-----------------------------------------------------------------------
// <copyright file="ScreenBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using Caliburn.Micro;

/// <summary>
/// Represents the screen base class.
/// </summary>
/// <seealso cref="Caliburn.Micro.Screen" />
public class ScreenBase : Screen
{
    #region Fields

    private readonly IWindowManager _windowManager;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenBase"/> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    public ScreenBase(IWindowManager windowManager)
    {
        _windowManager = windowManager;
    }

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override Task OnActivateAsync(CancellationToken token)
    {
        Execute.OnUIThread(() =>
        {
            _windowManager.ShowWindowAsync(this);
        });

        return base.OnActivateAsync(token);
    }

    #endregion
}