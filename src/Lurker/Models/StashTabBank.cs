//-----------------------------------------------------------------------
// <copyright file="StashTabBank.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using ConfOxide;

    /// <summary>
    /// Represetns the bank of tab.
    /// </summary>
    public sealed class StashTabBank : SettingsBase<StashTabBank>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StashTabBank"/> class.
        /// </summary>
        public StashTabBank()
        {
            this.Tabs = new List<StashTab>();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        public List<StashTab> Tabs { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Ge the tab.
        /// </summary>
        /// <param name="name">The anme of the tab.</param>
        /// <returns>The stash tab.</returns>
        public StashTab Get(string name) => this.Tabs.FirstOrDefault(t => t.Name == name);

        /// <summary>
        /// Check if tab exist.
        /// </summary>
        /// <param name="name">The tab name.</param>
        /// <returns>If exist.</returns>
        public bool Exist(string name) => this.Tabs.Any(t => t.Name == name);

        /// <summary>
        /// Add to list.
        /// </summary>
        /// <param name="tab">The tab.</param>
        public void AddOrUpdate(StashTab tab)
        {
            var existingTab = this.Tabs.FirstOrDefault(t => t.Name == tab.Name);
            if (existingTab != null)
            {
                existingTab.TabType = tab.TabType;
                existingTab.InFolder = tab.InFolder;
                return;
            }

            this.Tabs.Add(tab);
        }

        /// <summary>
        /// Remove the tab.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IF removed.</returns>
        public bool Remove(string name)
        {
            var tabsToRemove = new List<StashTab>();
            foreach (var tab in this.Tabs.Where(t => t.Name == name))
            {
                tabsToRemove.Add(tab);
            }

            foreach (var tab in tabsToRemove)
            {
                this.Tabs.Remove(tab);
            }

            return tabsToRemove.Any();
        }

        #endregion
    }
}