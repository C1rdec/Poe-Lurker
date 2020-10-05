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
        #region fields

        private List<Skill> _skills;
        private List<UniqueItem> _items;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Build"/> class.
        /// </summary>
        public Build()
        {
            this._skills = new List<Skill>();
            this._items = new List<UniqueItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skills.
        /// </summary>
        public IEnumerable<Skill> Skills => this._skills;

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<UniqueItem> Items => this._items;

        /// <summary>
        /// Gets or sets the tree URL.
        /// </summary>
        public string SkillTreeUrl { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        public void AddSkill(Skill skill)
        {
            this._skills.Add(skill);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(UniqueItem item)
        {
            if (this._items.Any(i => i.Name == item.Name))
            {
                return;
            }

            this._items.Add(item);
        }

        #endregion
    }
}