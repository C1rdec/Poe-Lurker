//-----------------------------------------------------------------------
// <copyright file="Collaboration.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System;

    public class Collaboration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the expire date.
        /// </summary>
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        public string Animation { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public string Image { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether this instance is expired.
        /// </summary>  
        public bool IsExpired()
        {
            return DateTime.Compare(DateTime.Now, this.ExpireDate) >= 0 ? true : false;
        }

        #endregion
    }
}
