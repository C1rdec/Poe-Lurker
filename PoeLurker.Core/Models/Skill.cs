//-----------------------------------------------------------------------
// <copyright file="Skill.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Represents a skill.
/// </summary>
public class Skill
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Skill"/> class.
    /// </summary>
    public Skill()
    {
        Gems = new List<Gem>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the skills.
    /// </summary>
    public IList<Gem> Gems { get; private set; }

    /// <summary>
    /// Gets or sets the slot.
    /// </summary>
    public string Slot { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Froms the XML.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="knownGems">The known gems.</param>
    /// <returns>
    /// The skill.
    /// </returns>
    public static Skill FromXml(XElement element, IEnumerable<Gem> knownGems)
    {
        var skill = new Skill()
        {
            Slot = element.Attribute("slot")?.Value,
        };

        foreach (var gemElement in element.Elements())
        {
            var attribute = gemElement.Attribute("skillId");
            if (attribute == null)
            {
                continue;
            }

            var gemId = attribute.Value;
            var gem = knownGems.FirstOrDefault(g => g.Id == gemId);
            if (gem == null)
            {
                gem = Gem.FromXml(gemElement);
                if (string.IsNullOrEmpty(gem.Name) || gem.Level == 0)
                {
                    continue;
                }
            }

            skill.AddGem(gem);
        }

        return skill;
    }

    /// <summary>
    /// Adds the skill.
    /// </summary>
    /// <param name="gem">The gem.</param>
    public void AddGem(Gem gem)
    {
        Gems.Add(gem);
    }

    /// <summary>
    /// Determines whether the specified, is equal to this instance.
    /// </summary>
    /// <param name="obj">The  to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        var skill = obj as Skill;
        if (skill == null)
        {
            return false;
        }

        if (skill.Gems.Count() != Gems.Count())
        {
            return false;
        }

        for (int i = 0; i < skill.Gems.Count(); i++)
        {
            var gem = skill.Gems.ElementAt(i);
            var myGem = Gems.ElementAt(i);

            if (gem.Id != myGem.Id || gem.Name != myGem.Name)
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}