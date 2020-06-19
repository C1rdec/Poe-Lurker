//-----------------------------------------------------------------------
// <copyright file="BuildViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Represents a build viewmodel.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class BuildViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly string FileName = "build.pob";
        private Task _currentTask;
        private bool _isVisible;
        private bool _hasNoBuild;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        public BuildViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            if (AssetService.Exists(FileName))
            {
                this._currentTask = this.Initialize(File.ReadAllText(AssetService.GetFilePath(FileName)));
            }
            else
            {
                this._hasNoBuild = true;
            }

            this.IsVisible = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the skills.
        /// </summary>
        public IEnumerable<SkillViewModel> Skills { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }

            set
            {
                this._isVisible = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has build.
        /// </summary>
        public bool HasNoBuild
        {
            get
            {
                return this._hasNoBuild;
            }

            set
            {
                this._hasNoBuild = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Imports this instance.
        /// </summary>
        public async void Import()
        {
            try
            {
                if (this._currentTask != null)
                {
                    await this._currentTask;
                }

                var text = ClipboardHelper.GetClipboardText();
                if (System.Uri.IsWellFormedUriString(text, System.UriKind.RelativeOrAbsolute))
                {
                    var url = new Uri(text);
                    var rawUri = new Uri($"https://pastebin.com/raw{url.AbsolutePath}");
                    using (var client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, rawUri);
                        var response = await client.SendAsync(request);
                        text = await response.Content.ReadAsStringAsync();
                    }
                }

                if (await this.Initialize(text))
                {
                    AssetService.Create(FileName, text);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Toggles this instance.
        /// </summary>
        public void Toggle()
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// Initializes the specified build value.
        /// </summary>
        /// <param name="buildValue">The build value.</param>
        /// <returns>Representing the asynchronous operation.</returns>
        public async Task<bool> Initialize(string buildValue)
        {
            try
            {
                using (var service = new PathOfBuildingService())
                {
                    await service.InitializeAsync();
                    var build = service.Decode(buildValue);

                    this.Skills = build.Skills.Select(s => new SkillViewModel(s));
                    this.NotifyOfPropertyChange("Skills");
                }
            }
            catch
            {
                return false;
            }

            this.HasNoBuild = false;
            return true;
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = 220 * windowInformation.Height / 1080;
            var margin = PoeApplicationContext.WindowStyle == WindowStyle.Windowed ? 10 : 0;
            Execute.OnUIThread(() =>
            {
                this.View.Height = 500;
                this.View.Width = 350;
                this.View.Left = windowInformation.Position.Right - 350 - margin;
                this.View.Top = windowInformation.Position.Bottom - value - 500 - margin;
            });
        }

        #endregion
    }
}