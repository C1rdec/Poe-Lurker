//-----------------------------------------------------------------------
// <copyright file="Build.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a build.
    /// </summary>
    public class Build
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Build"/> class.
        /// </summary>
        public Build()
        {
            this.Skills = new List<Skill>();
            this.Items = new List<UniqueItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skills.
        /// </summary>
        public IList<Skill> Skills { get; private set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IList<UniqueItem> Items { get; private set; }

        /// <summary>
        /// Gets or sets the tree URL.
        /// </summary>
        public string SkillTreeUrl { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the ascendancy.
        /// </summary>
        /// <value>The ascendancy.</value>
        public string Ascendancy { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(UniqueItem item)
        {
            if (this.Items.Any(i => i.Name == item.Name))
            {
                return;
            }

            this.Items.Add(item);
        }

        #endregion
    }
}