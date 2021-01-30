//-----------------------------------------------------------------------
// <copyright file="BuildConfigurationViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Class BuildConfigurationViewModel.
    /// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class BuildConfigurationViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private static readonly PathOfBuildingService PathOfBuildingService = new PathOfBuildingService();
        private Build _build;
        private SimpleBuild _buildConfiguration;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurationViewModel" /> class.
        /// </summary>
        /// <param name="build">The build.</param>
        public BuildConfigurationViewModel(SimpleBuild build)
        {
            this._buildConfiguration = build;
            this.Items = new ObservableCollection<UniqueItemViewModel>();
            if (PathOfBuildingService.IsInitialize)
            {
                this.DecodeBuild(build);
            }
            else
            {
                PathOfBuildingService.InitializeAsync().ContinueWith((t) =>
                {
                    this.DecodeBuild(build);
                });
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
        /// Gets the items.
        /// </summary>
        public ObservableCollection<UniqueItemViewModel> Items { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string DisplayName => this._build == null ? string.Empty : $"{this._build.Class} ({this._build.Ascendancy})";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string BuildName
        {
            get
            {
                return this._buildConfiguration.Name;
            }

            set
            {
                this._buildConfiguration.Name = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the youtube.
        /// </summary>
        public string Youtube
        {
            get
            {
                return this._buildConfiguration.YoutubeUrl;
            }

            set
            {
                this._buildConfiguration.YoutubeUrl = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the forum post.
        /// </summary>
        public string Forum
        {
            get
            {
                return this._buildConfiguration.ForumUrl;
            }

            set
            {
                this._buildConfiguration.ForumUrl = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id => this._buildConfiguration.Id;

        #endregion

        #region Methods

        /// <summary>
        /// Decodes the build.
        /// </summary>
        /// <param name="simpleBuild">The simple build.</param>
        private void DecodeBuild(SimpleBuild simpleBuild)
        {
            this._build = PathOfBuildingService.Decode(simpleBuild.PathOfBuildingCode);
            this.NotifyOfPropertyChange("DisplayName");
            var mainSkill = this._build.Skills.OrderByDescending(s => s.Gems.Count(g => g.Support)).FirstOrDefault();
            if (mainSkill != null)
            {
                var gem = mainSkill.Gems.FirstOrDefault(g => !g.Support);
                if (gem != null)
                {
                    this.GemViewModel = new GemViewModel(gem, false);
                    this.NotifyOfPropertyChange("GemViewModel");
                }
            }

            foreach (var item in this._build.Items.OrderBy(i => i.Level))
            {
                this.Items.Add(new UniqueItemViewModel(item, false));
            }
        }

        #endregion
    }
}