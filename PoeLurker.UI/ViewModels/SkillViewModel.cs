//-----------------------------------------------------------------------
// <copyright file="SkillViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Core.Models;
using PoeLurker.UI.Models;

/// <summary>
/// Represents a skill viewmodel.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class SkillViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly Skill _skill;
    private bool _selected;
    private bool _selectable;
    private bool _justChecked;
    private readonly IEventAggregator _eventAggregator;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillViewModel" /> class.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public SkillViewModel(Skill skill)
        : this(skill, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillViewModel" /> class.
    /// </summary>
    /// <param name="skill">The skill.</param>
    /// <param name="selectable">if set to <c>true</c> [selectable].</param>
    public SkillViewModel(Skill skill, bool selectable)
    {
        _skill = skill;
        _selectable = selectable;
        _eventAggregator = IoC.Get<IEventAggregator>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the skill.
    /// </summary>
    public Skill Skill => _skill;

    /// <summary>
    /// Gets a value indicating whether this <see cref="SkillViewModel"/> is selectable.
    /// </summary>
    public bool Selectable
    {
        get
        {
            return _selectable;
        }

        private set
        {
            _selectable = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="SkillViewModel"/> is selected.
    /// </summary>
    public bool Selected
    {
        get
        {
            return _selected;
        }

        set
        {
            if (_selected != value)
            {
                _selected = value;
                NotifyOfPropertyChange();
            }
        }
    }

    /// <summary>
    /// Gets the gems.
    /// </summary>
    public IEnumerable<GemViewModel> Gems => _skill.Gems.OrderBy(g => g.Support).Select(g => new GemViewModel(g));

    #endregion

    #region Methods

    /// <summary>
    /// Called when [click].
    /// </summary>
    public void OnClick()
    {
        if (_justChecked)
        {
            _justChecked = false;
            if (Selected)
            {
                _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Skill = _skill });
            }

            return;
        }

        Selected = false;
        _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Skill = _skill, Delete = true });
    }

    /// <summary>
    /// Called when [checked].
    /// </summary>
    public void OnChecked()
    {
        _justChecked = true;
    }

    /// <summary>
    /// Sets the selectable.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public void SetSelectable(bool value)
    {
        Selectable = value;
    }

    #endregion
}