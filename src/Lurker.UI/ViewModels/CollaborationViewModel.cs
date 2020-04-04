//-----------------------------------------------------------------------
// <copyright file="CollaborationViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Models;

    public class CollaborationViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Collaboration _collaboration;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollaborationViewModel"/> class.
        /// </summary>
        /// <param name="collaboration">The collaboration.</param>
        public CollaborationViewModel(Collaboration collaboration)
        {
            this._collaboration = collaboration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has animation.
        /// </summary>
        public bool HasAnimation => !string.IsNullOrEmpty(this._collaboration.Animation);

        /// <summary>
        /// Gets a value indicating whether this instance has image.
        /// </summary>
        public bool HasImage => !string.IsNullOrEmpty(this._collaboration.Image);

        #endregion
    }
}
