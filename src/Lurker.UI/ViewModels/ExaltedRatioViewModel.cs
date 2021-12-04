//-----------------------------------------------------------------------
// <copyright file="ExaltedRatioViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    /// <summary>
    /// Represents the Exalted Ratio.
    /// </summary>
    public class ExaltedRatioViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExaltedRatioViewModel"/> class.
        /// </summary>
        /// <param name="ratio">The ratio.</param>
        public ExaltedRatioViewModel(double ratio)
        {
            this.Ratio = ratio;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ratio.
        /// </summary>
        public double Ratio { get; private set; }

        #endregion
    }
}