//-----------------------------------------------------------------------
// <copyright file="StashTabService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using PoeLurker.Core.Models;
using PoeLurker.Patreon.Models;

/// <summary>
/// Represents the stash tab service.
/// </summary>
public class StashTabService
{
    #region Fields

    private readonly StashTabBank _tabBank;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StashTabService"/> class.
    /// </summary>
    public StashTabService()
    {
        _tabBank = new StashTabBank();
        _tabBank.Initialize();
    }

    #endregion

    #region Properties

    #endregion

    #region Events

    /// <summary>
    /// The event.
    /// </summary>
    public event EventHandler<StashTabLocation> NewMarkerRequested;

    /// <summary>
    /// The close event.
    /// </summary>
    public event EventHandler CloseRequested;

    #endregion

    #region Methods

    /// <summary>
    /// Place the marker.
    /// </summary>
    /// <param name="location">The location.</param>
    public void PlaceMarker(Location location)
    {
        StashTabLocation tabLocation;
        var tab = _tabBank.Get(location.StashTabName);
        if (tab == null)
        {
            tabLocation = new StashTabLocation()
            {
                Name = location.StashTabName,
                StashTabType = StashTabType.Regular,
                InFolder = false,
                Left = location.Left,
                Top = location.Top,
            };
        }
        else
        {
            tabLocation = new StashTabLocation()
            {
                Name = location.StashTabName,
                StashTabType = tab.TabType,
                InFolder = tab.InFolder,
                Left = location.Left,
                Top = location.Top,
            };
        }

        NewMarkerRequested?.Invoke(this, tabLocation);
    }

    /// <summary>
    /// Close the stash tab.
    /// </summary>
    public void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Add to tab.
    /// </summary>
    /// <param name="name">The name.</param>
    public void AddQuadTab(string name)
    {
        _tabBank.AddOrUpdate(new StashTab() { Name = name, TabType = StashTabType.Quad });
        Save();
    }

    /// <summary>
    /// Remove from tab.
    /// </summary>
    /// <param name="name">The name.</param>
    public void RemoveQuadTab(string name)
    {
        _tabBank.AddOrUpdate(new StashTab() { Name = name, TabType = StashTabType.Regular });
        Save();
    }

    /// <summary>
    /// Update the tab.
    /// </summary>
    /// <param name="tab">The tab.</param>
    public void AddOrUpdateTab(StashTab tab)
    {
        _tabBank.AddOrUpdate(tab);
        Save();
    }

    /// <summary>
    /// Save the file.
    /// </summary>
    private void Save()
    {
        _tabBank.Save();
    }

    #endregion
}