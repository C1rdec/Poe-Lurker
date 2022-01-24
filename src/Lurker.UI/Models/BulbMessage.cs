//-----------------------------------------------------------------------
// <copyright file="BulbMessage.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using System;

    /// <summary>
    /// Represents a bulbMessage.
    /// </summary>
    public class BulbMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is sticky.
        /// </summary>
        public bool Sticky { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public Action SubAction { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        public System.ComponentModel.INotifyPropertyChanged View { get; set; }

        /// <summary>
        /// Gets or sets the on show.
        /// </summary>
        public Action<Action> OnShow { get; set; }

        /// <summary>
        /// Gets or sets the display time.
        /// </summary>
        public TimeSpan DisplayTime { get; set; }

        #endregion
    }
}