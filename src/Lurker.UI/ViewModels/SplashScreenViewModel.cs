//-----------------------------------------------------------------------
// <copyright file="SplashscreenViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.IO;
    using System.Reflection;
    using Lurker.Services;

    /// <summary>
    /// Represents a SplashScreen.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class SplashscreenViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private static readonly string LottieFileName = "LurckerIcon.json";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashscreenViewModel"/> class.
        /// </summary>
        public SplashscreenViewModel()
        {
            if (!AssetService.Exists(LottieFileName))
            {
                AssetService.Create(LottieFileName, GetResourceContent(LottieFileName));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the animation file path.
        /// </summary>
        public string AnimationFilePath => AssetService.GetFilePath(LottieFileName);

        #endregion

        #region Methods

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