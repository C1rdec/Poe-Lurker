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

    public class UpdateViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModel"/> class.
        /// </summary>
        public UpdateViewModel(bool needUpdate)
        {
            if (!File.Exists(this.NeedUpdateFilePath))
            {
                File.WriteAllText(this.NeedUpdateFilePath, this.GetResourceContent(this.NeedUpdateFileName));
            }

            if (!File.Exists(this.AnimationFilePath))
            {
                File.WriteAllText(this.UpdateSuccessFilePath, this.GetResourceContent(this.UpdateSuccessFileName));
            }

            if (needUpdate)
            {
                this.AnimationFilePath = this.NeedUpdateFilePath;
            }
            else
            {
                this.AnimationFilePath = this.UpdateSuccessFilePath;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        public string AnimationFilePath { get; private set; }

        /// <summary>
        /// Gets the need update file path.
        /// </summary>
        public string NeedUpdateFilePath => Path.Combine(this.SettingsFolderPath, this.NeedUpdateFileName);

        /// <summary>
        /// Gets the update success file path.
        /// </summary>
        public string UpdateSuccessFilePath => Path.Combine(this.SettingsFolderPath, this.UpdateSuccessFileName);

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        private string FolderName => "PoeLurker";

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        private string NeedUpdateFileName => "UpdateAnimation.json";

        /// <summary>
        /// Gets the name of the update success file.
        /// </summary>
        private string UpdateSuccessFileName => "UpdateSuccessAnimation.json";

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
        private string GetResourceContent(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Lurker.UI.Assets.{fileName}"))
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
