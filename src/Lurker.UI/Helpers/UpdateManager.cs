//-----------------------------------------------------------------------
// <copyright file="UpdateManager.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using Lurker.Services;
    using Squirrel;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class UpdateManager
    {
        #region Fields

        private static readonly string PoeLukerGithubUrl = "https://github.com/C1rdec/Poe-Lurker";
        private SettingsService _settingsService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        /// <param name="settingService">The setting service.</param>
        public UpdateManager(SettingsService settingsService)
        {
            this._settingsService = settingsService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public async Task Update()
        {
            this._settingsService.FirstLaunch = true;
            this._settingsService.Save();

            using (var updateManager = await Squirrel.UpdateManager.GitHubUpdateManager(PoeLukerGithubUrl))
            {
                await updateManager.UpdateApp();
                Squirrel.UpdateManager.RestartApp();
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>True if needs update</returns>
        public async Task<bool> CheckForUpdate()
        {
            try
            {
                using (var updateManager = await Squirrel.UpdateManager.GitHubUpdateManager(PoeLukerGithubUrl))
                {
                    var information = await updateManager.CheckForUpdate();
                    return information.ReleasesToApply.Any();
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
