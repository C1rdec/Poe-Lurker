//-----------------------------------------------------------------------
// <copyright file="NinjaResult.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models.Ninja
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a build.
    /// </summary>
    public class NinjaResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Lines.
        /// </summary>
        public IEnumerable<NinjaLine> Lines { get; set; }

        #endregion
    }
}