//-----------------------------------------------------------------------
// <copyright file="KeyCodeService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System.IO;
    using ConfOxide;
    using Lurker.Models;

    /// <summary>
    /// Represetns key code service.
    /// </summary>
    /// <seealso cref="Lurker.Services.ServiceBase" />
    public class KeyCodeService : ServiceBase
    {
        #region Fields

        private KeyCodeSettings _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCodeService"/> class.
        /// </summary>
        public KeyCodeService()
        {
            this._settings = new KeyCodeSettings();
            if (!File.Exists(this.FilePath))
            {
                using (var file = File.Create(this.FilePath))
                {
                }

                this.Save();
            }
            else
            {
                try
                {
                    this._settings.ReadJsonFile(this.FilePath);
                }
                catch
                {
                    File.Delete(this.FilePath);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the toggle build.
        /// </summary>
        public ushort ToggleBuild
        {
            get
            {
                return this._settings.ToggleBuild;
            }

            set
            {
                this._settings.ToggleBuild = value;
            }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        protected override string FileName => "KeyCode.json";

        #endregion

        #region Methods

        /// <summary>
        /// Saves the specified raise event.
        /// </summary>
        public void Save()
        {
            this._settings.WriteJsonFile(this.FilePath);
        }

        #endregion
    }
}