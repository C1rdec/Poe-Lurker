﻿namespace PoeLurker.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a build.
/// </summary>
public class Build
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Build"/> class.
    /// </summary>
    public Build()
    {
        Id = Guid.NewGuid();
        Skills = [];
        Items = [];
        SkillTrees = [];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the skills.
    /// </summary>
    public IList<Skill> Skills { get; private set; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IList<UniqueItem> Items { get; private set; }

    /// <summary>
    /// Gets or sets the tree URL.
    /// </summary>
    public List<SkillTreeInformation> SkillTrees { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the XML.
    /// </summary>
    public string Xml { get; set; }

    /// <summary>
    /// Gets or sets the class.
    /// </summary>
    /// <value>The class.</value>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the ascendancy.
    /// </summary>
    /// <value>The ascendancy.</value>
    public string Ascendancy { get; set; }

    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// Gets or sets the damage.
    /// </summary>
    public DamageValue Damage { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the skill.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public void AddSkill(Skill skill)
    {
        Skills.Add(skill);
    }

    /// <summary>
    /// Adds the item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void AddItem(UniqueItem item)
    {
        if (Items.Any(i => i.Name == item.Name))
        {
            return;
        }

        Items.Add(item);
    }

    #endregion
}