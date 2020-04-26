//-----------------------------------------------------------------------
// <copyright file="CollaborationViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Models;

    /// <summary>
    /// Represents the collaborationviewmodel.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
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
        /// Gets the name.
        /// </summary>
        public string Name => this._collaboration.Name;

        #endregion
    }
}