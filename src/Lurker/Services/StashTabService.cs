//-----------------------------------------------------------------------
// <copyright file="StashTabService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.IO;
    using ConfOxide;
    using Lurker.Models;
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represents the stash tab service.
    /// </summary>
    public class StashTabService : ServiceBase
    {
        #region Fields

        private StashTabBank _tabBank;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StashTabService"/> class.
        /// </summary>
        public StashTabService()
        {
            this._tabBank = new StashTabBank();
            if (!File.Exists(this.FilePath))
            {
                using (var file = File.Create(this.FilePath))
                {
                }

                this.Save();
            }
            else
            {
                try
                {
                    this._tabBank.ReadJsonFile(this.FilePath);
                }
                catch
                {
                    File.Delete(this.FilePath);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the File name.
        /// </summary>
        protected override string FileName => "Tab.json";

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
            var tab = this._tabBank.Get(location.StashTabName);
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

            this.NewMarkerRequested?.Invoke(this, tabLocation);
        }

        /// <summary>
        /// Close the stash tab.
        /// </summary>
        public void Close()
        {
            this.CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Add to tab.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AddQuadTab(string name)
        {
            this._tabBank.AddOrUpdate(new StashTab() { Name = name, TabType = StashTabType.Quad });
            this.Save();
        }

        /// <summary>
        /// Remove from tab.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveQuadTab(string name)
        {
            this._tabBank.AddOrUpdate(new StashTab() { Name = name, TabType = StashTabType.Regular });
            this.Save();
        }

        /// <summary>
        /// Update the tab.
        /// </summary>
        /// <param name="tab">The tab.</param>
        public void AddOrUpdateTab(StashTab tab)
        {
            this._tabBank.AddOrUpdate(tab);
            this.Save();
        }

        /// <summary>
        /// Save the file.
        /// </summary>
        private void Save()
        {
            this._tabBank.WriteJsonFile(this.FilePath);
        }

        #endregion
    }
}