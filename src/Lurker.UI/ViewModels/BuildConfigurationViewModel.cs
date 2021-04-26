//-----------------------------------------------------------------------
// <copyright file="BuildConfigurationViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
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
        private string _ascendency;

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
        /// Gets the simple build.
        /// </summary>
        public SimpleBuild SimpleBuild => this._buildConfiguration;

        /// <summary>
        /// Gets the ascendancy.
        /// </summary>
        public string Ascendancy
        {
            get
            {
                return this._ascendency;
            }

            private set
            {
                this._ascendency = value;
                this.NotifyOfPropertyChange();
            }
        }

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
                this.NotifyOfPropertyChange("HasBuildName");
                this.NotifyOfPropertyChange("HasNoBuildName");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has build name.
        /// </summary>
        public bool HasBuildName => !string.IsNullOrEmpty(this.BuildName);

        /// <summary>
        /// Gets a value indicating whether this instance has youtube.
        /// </summary>
        public bool HasYoutube => !string.IsNullOrEmpty(this.Youtube);

        /// <summary>
        /// Gets a value indicating whether this instance has forum.
        /// </summary>
        public bool HasForum => !string.IsNullOrEmpty(this.Forum);

        /// <summary>
        /// Gets a value indicating whether this instance has no build name.
        /// </summary>
        public bool HasNoBuildName => !this.HasBuildName;

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
                this.NotifyOfPropertyChange("HasYoutube");
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
                this.NotifyOfPropertyChange("HasForum");
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id => this._buildConfiguration.Id;

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get
            {
                return this._buildConfiguration.Notes;
            }

            set
            {
                this._buildConfiguration.Notes = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the tree.
        /// </summary>
        public void OpenTree()
        {
            if (this._build != null)
            {
                Process.Start(this._build.SkillTreeUrl);
            }
        }

        /// <summary>
        /// Opens the youtube.
        /// </summary>
        public void OpenYoutube()
        {
            OpenUrl(this.Youtube);
        }

        /// <summary>
        /// Opens the forum.
        /// </summary>
        public void OpenForum()
        {
            OpenUrl(this.Forum);
        }

        /// <summary>
        /// Opens the URL.
        /// </summary>
        /// <param name="value">The value.</param>
        private static void OpenUrl(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri _))
            {
                Process.Start(value);
            }
        }

        /// <summary>
        /// Decodes the build.
        /// </summary>
        /// <param name="simpleBuild">The simple build.</param>
        private void DecodeBuild(SimpleBuild simpleBuild)
        {
            this._build = PathOfBuildingService.Decode(simpleBuild.PathOfBuildingCode);
            this.Ascendancy = this._build.Ascendancy;
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

            Caliburn.Micro.Execute.OnUIThread(() =>
            {
                foreach (var item in this._build.Items.OrderBy(i => i.Level))
                {
                    this.Items.Add(new UniqueItemViewModel(item, false));
                }
            });
        }

        #endregion
    }
}