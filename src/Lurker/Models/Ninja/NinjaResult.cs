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
    /// <typeparam name="T">Generics.</typeparam>
    public class NinjaResult<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Lines.
        /// </summary>
        public IEnumerable<T> Lines { get; set; }

        #endregion
    }
}