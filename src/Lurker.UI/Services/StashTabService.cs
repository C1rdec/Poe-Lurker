//-----------------------------------------------------------------------
// <copyright file="StashTabService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Services
{
    using System;
    using Lurker.Patreon.Models;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the stash tab service.
    /// </summary>
    public class StashTabService
    {
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
        /// <param name="stashTqabType">The type.</param>
        public void PlaceMarker(int top, int left, StashTabType stashTqabType)
        {
            var location = new StashTabLocation()
            {
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
            this.PlaceMarker(location.Top, location.Left, StashTabType.Regular);
        }

        /// <summary>
        /// Close the stash tab.
        /// </summary>
        public void Close()
        {
            this.CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}