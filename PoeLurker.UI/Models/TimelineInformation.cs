namespace PoeLurker.UI.Models
{
    /// <summary>
    /// Repre3sent the time line information.
    /// </summary>
    public class TimelineInformation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double MaximumValue { get; set; }

        /// <summary>
        /// Gets or sets the actual width.
        /// </summary>
        public double ActualWidth { get; set; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public double CurrentValue { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the relative position.
        /// </summary>
        /// <returns>The position.</returns>
        public double GetRelativePosition()
        {
            return this.GetRelativePosition(this.CurrentValue);
        }

        /// <summary>
        /// Gets the relative position.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The position.</returns>
        public double GetRelativePosition(double value)
        {
            return (value / this.MaximumValue) * this.ActualWidth;
        }

        #endregion
    }
}