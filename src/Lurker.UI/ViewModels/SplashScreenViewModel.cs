//-----------------------------------------------------------------------
// <copyright file="SplashscreenViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.IO;
    using System.Reflection;
    using Caliburn.Micro;
    using Lurker.Patreon.Services;
    using Lurker.Services;

    /// <summary>
    /// Represents a SplashScreen.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class SplashscreenViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private static readonly string LottieFileName = "LurckerIcon.json";
        private SettingsViewModel _settings;
        private IEventAggregator _eventAggrator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashscreenViewModel" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SplashscreenViewModel(SettingsViewModel settings, IEventAggregator eventAggregator)
        {
            this._settings = settings;
            this._eventAggrator = eventAggregator;
            if (!AssetService.Exists(LottieFileName))
            {
                AssetService.Create(LottieFileName, GetResourceContent(LottieFileName));
            }

            Execute.OnUIThread(async () =>
            {
                using (var service = new PatreonService())
                {
                    var result = await service.IsPledging();
                    if (result)
                    {
                        return;
                    }

                    this.ShowPatreon = true;
                    this.TrialAvailable = service.TrialAvailable;
                    if (!this.TrialAvailable)
                    {
                        // this.ShowPatreon = true;
                    }
                }
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the animation file path.
        /// </summary>
        public string AnimationFilePath => AssetService.GetFilePath(LottieFileName);

        /// <summary>
        /// Gets or sets a value indicating whether [trial available].
        /// </summary>
        public bool TrialAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show patreon].
        /// </summary>
        public bool ShowPatreon { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the lurker pro.
        /// </summary>
        public void OpenLurkerPro()
        {
            // Set to luker pro tab
            this._settings.OpenLurkerPro();
            this._eventAggrator.PublishOnUIThread(this._settings);
        }

        /// <summary>
        /// Gets the content of the resource.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The animation text.
        /// </returns>
        private static string GetResourceContent(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Lurker.UI.Assets.{fileName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion
    }
}