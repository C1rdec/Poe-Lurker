//-----------------------------------------------------------------------
// <copyright file="CollaborationViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
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
        /// Gets the name.
        /// </summary>
        public string Name => this._collaboration.Name;

        #endregion
    }
}
