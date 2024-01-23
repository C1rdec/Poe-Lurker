//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionExtension.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represetns the extension for the ObservableCollection.
    /// </summary>
    public static class ObservableCollectionExtension
    {
        #region Methods

        /// <summary>
        /// Sorts the specified comparison.
        /// </summary>
        /// <typeparam name="T">The type of the Collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="comparison">The comparison.</param>
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        #endregion
    }
}