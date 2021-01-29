//-----------------------------------------------------------------------
// <copyright file="BuildConfigurationViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Linq;
    using Lurker.Models;
    using Lurker.UI.Models;

    /// <summary>
    /// Class BuildConfigurationViewModel.
    /// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class BuildConfigurationViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Build _build;
        private BuildManagerContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurationViewModel" /> class.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="context">The context.</param>
        public BuildConfigurationViewModel(Build build, BuildManagerContext context)
        {
            this._build = build;
            this._context = context;
            var mainSkill = build.Skills.OrderByDescending(s => s.Gems.Count(g => g.Support)).FirstOrDefault();
            if (mainSkill != null)
            {
                var gem = mainSkill.Gems.FirstOrDefault(g => !g.Support);
                if (gem != null)
                {
                    this.GemViewModel = new GemViewModel(gem);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the gem view model.
        /// </summary>
        /// <value>The gem view model.</value>
        public GemViewModel GemViewModel { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => $"{this._build.Class} ({this._build.Ascendancy})";

        #endregion
    }
}