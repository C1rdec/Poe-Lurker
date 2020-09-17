//-----------------------------------------------------------------------
// <copyright file="TimelineController.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Services
{
    using System.Runtime.CompilerServices;
    using Lurker.UI.ViewModels;

    /// <summary>
    /// Represents the timeline controller.
    /// </summary>
    public static class TimelineController
    {
        #region Fields

        private static TimelineItemViewModel _currentItem;

        #endregion

        #region Methods

        /// <summary>
        /// Opens the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public static void Open(TimelineItemViewModel item)
        {
            Close();

            item.IsOpen = true;
            _currentItem = item;
        }

        /// <summary>
        /// Closes the current item.
        /// </summary>
        public static void Close()
        {
            if (_currentItem != null)
            {
                _currentItem.IsOpen = false;
                _currentItem = null;
            }
        }

        #endregion
    }
}