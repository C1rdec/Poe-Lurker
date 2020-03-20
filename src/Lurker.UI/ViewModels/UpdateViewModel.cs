//-----------------------------------------------------------------------
// <copyright file="UpdateViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.IO;
    using System.Reflection;

    public class UpdateViewModel: Caliburn.Micro.PropertyChangedBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModel"/> class.
        /// </summary>
        public UpdateViewModel()
        {
            if (!File.Exists(this.AnimationFilePath))
            {
                File.WriteAllText(this.AnimationFilePath, this.GetResourceContent());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        public string AnimationFilePath => Path.Combine(this.SettingsFolderPath, this.FileName);

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        private string FolderName => "PoeLurker";

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        private string FileName => "UpdateAnimation.json";

        /// <summary>
        /// Gets the application data folder path.
        /// </summary>
        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        private string SettingsFolderPath => Path.Combine(AppDataFolderPath, FolderName);

        #endregion

        #region Methods

        /// <summary>
        /// Gets the content of the resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The animation text.</returns>
        private string GetResourceContent()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Lurker.UI.Assets.UpdateAnimation.json"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion
    }
}
