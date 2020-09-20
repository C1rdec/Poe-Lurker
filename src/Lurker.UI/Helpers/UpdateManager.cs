//-----------------------------------------------------------------------
// <copyright file="UpdateManager.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Lurker.Services;
    using Squirrel;

    /// <summary>
    /// Represents the update manager.
    /// </summary>
    public class UpdateManager
    {
        #region Fields

        private static readonly string PoeLukerGithubUrl = "https://github.com/C1rdec/Poe-Lurker";
        private SettingsService _settingsService;
        private ClipboardLurker _clipboardLurker;
        private ClientLurker _clientLurker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager" /> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="clipboardLurker">The clipboard lurker.</param>
        /// <param name="clientLurker">The client lurker.</param>
        public UpdateManager(SettingsService settingsService, ClipboardLurker clipboardLurker, ClientLurker clientLurker)
        {
            this._clipboardLurker = clipboardLurker;
            this._clientLurker = clientLurker;
            this._settingsService = settingsService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>The task awaiter.</returns>
        public async Task Update()
        {
            this._settingsService.FirstLaunch = true;
            this._settingsService.Save();

            if (this._clipboardLurker != null)
            {
                this._clipboardLurker.Dispose();
            }

            if (this._clientLurker != null)
            {
                this._clientLurker.Dispose();
            }

            using (var updateManager = await Squirrel.UpdateManager.GitHubUpdateManager(PoeLukerGithubUrl))
            {
                await updateManager.UpdateApp();
                Squirrel.UpdateManager.RestartApp();
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>True if needs update.</returns>
        public async Task<bool> CheckForUpdate()
        {
            return false;
#if DEBUG
            return false;
#endif

#pragma warning disable CS0162
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
#pragma warning restore CS0162
        }

        #endregion
    }
}