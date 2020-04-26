//-----------------------------------------------------------------------
// <copyright file="AssetService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents the asset service.
    /// </summary>
    public static class AssetService
    {
        #region Properties

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        public static string SettingsFolderPath => Path.Combine(AppDataFolderPath, FolderName);

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        private static string FolderName => "PoeLurker";

        /// <summary>
        /// Gets the application data folder path.
        /// </summary>
        private static string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        #endregion

        #region Methods

        /// <summary>
        /// Gets the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The file content.</returns>
        public static string Get(string fileName)
        {
            return File.ReadAllText(Path.Combine(SettingsFolderPath, fileName));
        }

        /// <summary>
        /// Existses the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>If the asset exist.</returns>
        public static bool Exists(string fileName)
        {
            return File.Exists(Path.Combine(SettingsFolderPath, fileName));
        }

        /// <summary>
        /// Deletes the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void Delete(string fileName)
        {
            File.Delete(Path.Combine(SettingsFolderPath, fileName));
        }

        /// <summary>
        /// Creates the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">The content.</param>
        public static void Create(string fileName, string content)
        {
            File.WriteAllText(Path.Combine(SettingsFolderPath, fileName), content);
        }

        /// <summary>
        /// Updates the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">The content.</param>
        public static void Update(string fileName, string content)
        {
            if (Exists(fileName))
            {
                Delete(fileName);
            }

            Create(fileName, content);
        }

        #endregion
    }
}