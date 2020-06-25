//-----------------------------------------------------------------------
// <copyright file="PathOfExileProcessLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the Path of Exile process lurker.
    /// </summary>
    /// <seealso cref="Lurker.ProcessLurker" />
    public class PathOfExileProcessLurker : ProcessLurker
    {
        #region Fields

        private static readonly List<string> PossibleProcessNames = new List<string> { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam", "PathOfExile_x64_KG.exe", "PathOfExile_KG.exe" };
        private static readonly string WindowTitle = "Path of Exile";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathOfExileProcessLurker"/> class.
        /// </summary>
        public PathOfExileProcessLurker()
            : base(PossibleProcessNames)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Waits for process.
        /// </summary>
        /// <returns>
        /// The Path of Exile process.
        /// </returns>
        public override async Task<int> WaitForProcess()
        {
            var processId = await base.WaitForProcess();
            var process = GetProcessById(processId);

            // This is to filter the update window
            while (process == null || process.MainWindowTitle != WindowTitle)
            {
                processId = await base.WaitForProcess();
                process = GetProcessById(processId);
            }

            return processId;
        }

        #endregion
    }
}