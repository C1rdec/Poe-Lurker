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
        /// Check if tab exist.
        /// </summary>
        /// <param name="name">The tab name.</param>
        /// <returns>If exist.</returns>
        public bool Exist(string name) => this.Tabs.Any(t => t.Name == name);

        /// <summary>
        /// Add to list.
        /// </summary>
        /// <param name="name">The tabName.</param>
        public void Add(string name)
        {
            if (this.Exist(name))
            {
                return;
            }

            this.Tabs.Add(new StashTab() { Name = name, TabType = StashTabType.Quad });
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