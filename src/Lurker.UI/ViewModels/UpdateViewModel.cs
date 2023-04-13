//-----------------------------------------------------------------------
// <copyright file="UpdateViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.IO;
    using System.Reflection;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the update view model.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class UpdateViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private UpdateState _state;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModel" /> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public UpdateViewModel(UpdateState state)
        {
            this._state = state;

            if (!File.Exists(this.NeedUpdateFilePath))
            {
                File.WriteAllText(this.NeedUpdateFilePath, this.GetResourceContent(this.NeedUpdateFileName));
            }

            if (!File.Exists(this.UpdateSuccessFilePath))
            {
                File.WriteAllText(this.UpdateSuccessFilePath, this.GetResourceContent(this.UpdateSuccessFileName));
            }

            if (!File.Exists(this.UpdateWorkingFilePath))
            {
                File.WriteAllText(this.UpdateWorkingFilePath, this.GetResourceContent(this.UpdateWorkingFileName));
            }

            switch (state)
            {
                case UpdateState.NeedUpdate:
                    this.FilePath = "/Assets/Update.png";
                    break;
                case UpdateState.Success:
                    this.FilePath = "/Assets/Success.png";
                    break;
                case UpdateState.Working:
                    this.FilePath = "/Assets/Working.png";
                    break;
                default:
                    throw new System.NotSupportedException();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether gets working state.
        /// </summary>
        public bool IsWorking => this._state == UpdateState.Working;

        /// <summary>
        /// Gets a value indicating whether gets working state.
        /// </summary>
        public bool IsNotWorking => !this.IsWorking;

        /// <summary>
        /// Gets the need update file path.
        /// </summary>
        public string NeedUpdateFilePath => Path.Combine(this.SettingsFolderPath, this.NeedUpdateFileName);

        /// <summary>
        /// Gets the update success file path.
        /// </summary>
        public string UpdateSuccessFilePath => Path.Combine(this.SettingsFolderPath, this.UpdateSuccessFileName);

        /// <summary>
        /// Gets the update success file path.
        /// </summary>
        public string UpdateWorkingFilePath => Path.Combine(this.SettingsFolderPath, this.UpdateWorkingFileName);

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
        /// Gets the name of the update working file.
        /// </summary>
        private string UpdateWorkingFileName => "UpdateWorkingAnimation.json";

        /// <summary>
        /// Gets the application data folder path.
        /// </summary>
        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        private string SettingsFolderPath => Path.Combine(this.AppDataFolderPath, this.FolderName);

        #endregion

        #region Methods

        /// <summary>
        /// Gets the content of the resource.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// The animation text.
        /// </returns>
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