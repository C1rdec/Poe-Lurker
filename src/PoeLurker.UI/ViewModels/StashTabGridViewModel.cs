//-----------------------------------------------------------------------
// <copyright file="StashTabGridViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using ProcessLurker;

/// <summary>
/// Represents the stash tab grid.
/// #MagicNumbersLand.
/// </summary>
internal class StashTabGridViewModel : PoeOverlayBase, IDisposable
{
    #region Fields

    private static readonly int DefaultSize = 637;
    private static readonly int DefaultTabHeight = 1119;

    private static readonly int DefaultLeftMargin = 17;
    private static readonly int DefaultTopMargin = 154;

    private readonly StashTabService _service;

    private int _top;
    private int _left;
    private bool _isRegularTab;
    private bool _isInFolder;
    private bool _isVisible;
    private string _currentTabName;
    private PoeWindowInformation _currentWindowInformation;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="StashTabGridViewModel" /> class.
    /// </summary>
    /// <param name="stashTabService">The stash tab service.</param>
    /// <param name="windowManager">The window manage.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">the settings service.</param>
    public StashTabGridViewModel(StashTabService stashTabService, IWindowManager windowManager, DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService)
        : base(windowManager, dockingHelper, processLurker, settingsService)
    {
        _isRegularTab = true;
        _service = stashTabService;
        _service.NewMarkerRequested += Service_NewMarkerRequested;
        _service.CloseRequested += Service_CloseRequested;
    }

    #region Properties

    /// <summary>
    /// Gets Top.
    /// </summary>
    public int Top
    {
        get
        {
            return _top;
        }

        private set
        {
            _top = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets Left.
    /// </summary>
    public int Left
    {
        get
        {
            return _left;
        }

        private set
        {
            _left = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether IsVisible.
    /// </summary>
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }

        private set
        {
            _isVisible = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether IsRegularTab.
    /// </summary>
    public bool IsRegularTab
    {
        get
        {
            return _isRegularTab;
        }

        private set
        {
            _isRegularTab = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => IsQuadTab);
        }
    }

    /// <summary>
    /// Gets a value indicating whether IsQuadTab.
    /// </summary>
    public bool IsQuadTab => !IsRegularTab;

    /// <summary>
    /// Gets a value indicating whether the tab is in a folder.
    /// </summary>
    public bool IsInFolder
    {
        get
        {
            return _isInFolder;
        }

        private set
        {
            _isInFolder = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => IsNotInFolder);
        }
    }

    /// <summary>
    /// Gets a value indicating whether IsNotInFolder.
    /// </summary>
    public bool IsNotInFolder => !IsInFolder;

    #endregion

    #region Methods

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Toggle the tab type.
    /// </summary>
    public void ToggleTabType()
    {
        IsRegularTab = !IsRegularTab;

        if (IsQuadTab)
        {
            _service.AddQuadTab(_currentTabName);
        }
        else
        {
            _service.RemoveQuadTab(_currentTabName);
        }
    }

    /// <summary>
    /// Toggle the tab type.
    /// </summary>
    public void ToggleInFolder()
    {
        IsInFolder = !IsInFolder;
        _service.AddOrUpdateTab(new StashTab()
        {
            Name = _currentTabName,
            InFolder = IsInFolder,
            TabType = IsRegularTab ? StashTabType.Regular : StashTabType.Quad,
        });

        SetWindowPosition();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _service.NewMarkerRequested -= Service_NewMarkerRequested;
            _service.CloseRequested -= Service_CloseRequested;
        }
    }

    /// <summary>
    /// Will place the overlay.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        _currentWindowInformation = windowInformation;

        // When Poe Lurker is updated we save the settings before the view are loaded
        if (View == null)
        {
            return;
        }

        Execute.OnUIThread(() =>
        {
            var size = DefaultSize * windowInformation.Height / DefaultTabHeight;
            var leftMargin = DefaultLeftMargin * windowInformation.Height / DefaultTabHeight;
            var topMargin = DefaultTopMargin * windowInformation.Height / DefaultTabHeight;

            var top = windowInformation.Position.Top + topMargin - Margin;
            if (IsInFolder)
            {
                top += 42 * windowInformation.Height / DefaultTabHeight;
            }

            // 50 is the footer
            View.Height = ApplyAbsoluteScalingY(size + 50);
            View.Width = ApplyAbsoluteScalingX(size);
            View.Left = ApplyScalingX(windowInformation.Position.Left + leftMargin);
            View.Top = ApplyScalingY(top);
        });
    }

    private void SetWindowPosition() => SetWindowPosition(_currentWindowInformation);

    private void Service_NewMarkerRequested(object sender, StashTabLocation e)
    {
        _currentTabName = e.Name;
        Left = e.Left - 1;
        Top = e.Top - 1;
        IsRegularTab = e.StashTabType == StashTabType.Regular;
        IsVisible = true;
    }

    private void Service_CloseRequested(object sender, System.EventArgs e)
    {
        IsVisible = false;
    }

    #endregion
}