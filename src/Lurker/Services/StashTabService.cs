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
        /// <param name="top">The top position.</param>
        /// <param name="left">The left postion.</param>
        /// <param name="name">The name.</param>
        /// <param name="stashTqabType">The type.</param>
        public void PlaceMarker(int top, int left, string name, StashTabType stashTqabType)
        {
            var location = new StashTabLocation()
            {
                Name = name,
                StashTabType = stashTqabType,
                Left = left,
                Top = top,
            };

            this.NewMarkerRequested?.Invoke(this, location);
        }

        /// <summary>
        /// Place the marker.
        /// </summary>
        /// <param name="location">The location.</param>
        public void PlaceMarker(Location location)
        {
            var tabType = this._tabBank.Exist(location.StashTabName) ? StashTabType.Quad : StashTabType.Regular;
            this.PlaceMarker(location.Top, location.Left, location.StashTabName, tabType);
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
            this._tabBank.Add(name);
            this.Save();
        }

        /// <summary>
        /// Remove from tab.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveQuadTab(string name)
        {
            if (this._tabBank.Remove(name))
            {
                this.Save();
            }
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