//-----------------------------------------------------------------------
// <copyright file="TimelineMessage.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    /// <summary>
    /// Represents a timeline message.
    /// </summary>
    public abstract class TimelineMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SkillMessage"/> is clear.
        /// </summary>
        public bool Clear { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SkillMessage"/> is delete.
        /// </summary>
        public bool Delete { get; set; }

        #endregion
    }
}