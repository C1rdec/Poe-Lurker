//-----------------------------------------------------------------------
// <copyright file="SkillMessage.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using Lurker.Models;

    /// <summary>
    /// Represents the skill message.
    /// </summary>
    public class SkillMessage
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

        /// <summary>
        /// Gets or sets the skill.
        /// </summary>
        /// <value>
        /// The skill.
        /// </value>
        public Skill Skill { get; set; }

        #endregion
    }
}