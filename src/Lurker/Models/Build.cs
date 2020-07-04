//-----------------------------------------------------------------------
// <copyright file="Build.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a build.
    /// </summary>
    public class Build
    {
        #region fields

        private List<Skill> _skills;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Build"/> class.
        /// </summary>
        public Build()
        {
            this._skills = new List<Skill>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skills.
        /// </summary>
        public IEnumerable<Skill> Skills => this._skills;

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

        #endregion
    }
}