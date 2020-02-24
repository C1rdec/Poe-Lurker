//-----------------------------------------------------------------------
// <copyright file="LifeBulbMessage.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using System;

    public class LifeBulbMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        public System.ComponentModel.INotifyPropertyChanged View { get; set; }

        /// <summary>
        /// Gets or sets the on show.
        /// </summary>
        public Action<Action> OnShow { get; set; }

        #endregion
    }
}
