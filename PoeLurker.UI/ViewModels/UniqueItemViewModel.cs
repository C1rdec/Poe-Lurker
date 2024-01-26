//-----------------------------------------------------------------------
// <copyright file="UniqueItemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using Caliburn.Micro;
using PoeLurker.Core.Models;
using PoeLurker.Patreon.Models;
using PoeLurker.UI.Models;

/// <summary>
/// Represents a unique item.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class UniqueItemViewModel : WikiItemBaseViewModel
{
    #region Fields

    private readonly UniqueItem _item;
    private bool _selectable;
    private bool _selected;
    private bool _justChecked;
    private readonly IEventAggregator _eventAggregator;
    private readonly Action<UniqueItem> _onClick;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueItemViewModel"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="onClick">The onclick call back.</param>
    public UniqueItemViewModel(UniqueItem item, Action<UniqueItem> onClick)
        : this(item, false)
    {
        _onClick = onClick;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueItemViewModel" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="selectable">if set to <c>true</c> [selectable].</param>
    public UniqueItemViewModel(UniqueItem item, bool selectable)
        : base(item)
    {
        _selectable = selectable;
        _item = item;
        _eventAggregator = IoC.Get<IEventAggregator>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the item.
    /// </summary>
    public UniqueItem Item => _item;

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
    /// Gets the default image.
    /// </summary>
    public override Uri DefaultImage => GetDefaultImage();

    #endregion

    #region Methods

    /// <summary>
    /// Called when [click].
    /// </summary>
    public void OnClick()
    {
        if (!_selectable)
        {
            _onClick?.Invoke(Item);
            return;
        }

        if (_justChecked)
        {
            _justChecked = false;
            if (Selected)
            {
                _eventAggregator.PublishOnUIThreadAsync(new ItemMessage() { Item = _item });
            }

            return;
        }

        Selected = false;
        _eventAggregator.PublishOnUIThreadAsync(new ItemMessage() { Item = _item, Delete = true });
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

    /// <summary>
    /// Gets the default image.
    /// </summary>
    /// <returns>The default image Uri.</returns>
    private Uri GetDefaultImage()
    {
        switch (_item.ItemClass)
        {
            case ItemClass.Amulet:
            case ItemClass.Ring:
            case ItemClass.Belt:
            case ItemClass.Quiver:
                return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/4/45/Accessory_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074925");
            case ItemClass.BodyArmour:
            case ItemClass.Boots:
            case ItemClass.Gloves:
            case ItemClass.Helmet:
            case ItemClass.Shield:
                return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/7/72/Armour_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074830");
            case ItemClass.Axe:
            case ItemClass.Bow:
            case ItemClass.Dagger:
            case ItemClass.Mace:
            case ItemClass.Sceptre:
            case ItemClass.Staff:
            case ItemClass.Sword:
            case ItemClass.Wand:
                return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/9/90/Weapon_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074907");
            case ItemClass.Flask:
                return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/a/a5/Flask_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074845");
            case ItemClass.Jewel:
                return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/2/26/Jewel_item_icon.png/revision/latest/scale-to-width-down/80?cb=20150427211503");
        }

        return default;
    }

    #endregion
}