//-----------------------------------------------------------------------
// <copyright file="GroupItemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represent a group of items.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class GroupItemViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupItemViewModel"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public GroupItemViewModel(IEnumerable<WikiItemBaseViewModel> items)
        {
            this.Items = new ObservableCollection<WikiItemBaseViewModel>(items);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the items.
        /// </summary>
        public ObservableCollection<WikiItemBaseViewModel> Items { get; private set; }

        #endregion
    }
}