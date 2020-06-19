//-----------------------------------------------------------------------
// <copyright file="SkillViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Lurker.Models;

    /// <summary>
    /// Represents a skill viewmodel.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class SkillViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Skill _skill;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillViewModel"/> class.
        /// </summary>
        /// <param name="skill">The skill.</param>
        public SkillViewModel(Skill skill)
        {
            this._skill = skill;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the gems.
        /// </summary>
        public IEnumerable<GemViewModel> Gems => this._skill.Gems.Select(g => new GemViewModel(g));

        #endregion
    }
}