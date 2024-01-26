using Lurker.AppData;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PoeLurker.Core.Models;

internal class StashTabBankFile : AppDataFileBase<StashTabBank>
{
    protected override string FileName => "Tab.json";

    protected override string FolderName => "PoeLurker";

    #region Methods

    /// <summary>
    /// Ge the tab.
    /// </summary>
    /// <param name="name">The anme of the tab.</param>
    /// <returns>The stash tab.</returns>
    public StashTab Get(string name) => Entity.Tabs.FirstOrDefault(t => t.Name == name);

    /// <summary>
    /// Check if tab exist.
    /// </summary>
    /// <param name="name">The tab name.</param>
    /// <returns>If exist.</returns>
    public bool Exist(string name) => Entity.Tabs.Any(t => t.Name == name);

    /// <summary>
    /// Add to list.
    /// </summary>
    /// <param name="tab">The tab.</param>
    public void AddOrUpdate(StashTab tab)
    {
        var existingTab = Entity.Tabs.FirstOrDefault(t => t.Name == tab.Name);
        if (existingTab != null)
        {
            existingTab.TabType = tab.TabType;
            existingTab.InFolder = tab.InFolder;
            return;
        }

        Entity.Tabs.Add(tab);
    }

    /// <summary>
    /// Remove the tab.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>IF removed.</returns>
    public bool Remove(string name)
    {
        var tabsToRemove = new List<StashTab>();
        foreach (var tab in Entity.Tabs.Where(t => t.Name == name))
        {
            tabsToRemove.Add(tab);
        }

        foreach (var tab in tabsToRemove)
        {
            Entity.Tabs.Remove(tab);
        }

        return tabsToRemove.Any();
    }

    #endregion
}
