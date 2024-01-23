namespace PoeLurker.UI.Models
{
    using PoeLurker.Core.Models;

    /// <summary>
    /// Represents the skill message.
    /// </summary>
    public class SkillMessage : TimelineMessage
    {
        #region Properties

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