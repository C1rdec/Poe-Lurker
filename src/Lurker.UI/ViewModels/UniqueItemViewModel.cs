//-----------------------------------------------------------------------
// <copyright file="UniqueItemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Lurker.Models;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents a unique item.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class UniqueItemViewModel : WikiItemBaseViewModel
    {
        #region Fields

        private UniqueItem _item;
        private bool _selectable;
        private bool _selected;
        private bool _justChecked;
        private IEventAggregator _eventAggregator;
        private Action<UniqueItem> _onClick;

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
            this._onClick = onClick;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueItemViewModel" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="selectable">if set to <c>true</c> [selectable].</param>
        public UniqueItemViewModel(UniqueItem item, bool selectable)
            : base(item)
        {
            this._selectable = selectable;
            this._item = item;
            this._eventAggregator = IoC.Get<IEventAggregator>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item.
        /// </summary>
        public UniqueItem Item => this._item;

        /// <summary>
        /// Gets a value indicating whether this <see cref="SkillViewModel"/> is selectable.
        /// </summary>
        public bool Selectable
        {
            get
            {
                return this._selectable;
            }

            private set
            {
                this._selectable = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SkillViewModel"/> is selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                return this._selected;
            }

            set
            {
                if (this._selected != value)
                {
                    this._selected = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        public override Uri DefaultImage => this.GetDefaultImage();

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        public void OnClick()
        {
            if (!this._selectable)
            {
                this._onClick?.Invoke(this.Item);
                return;
            }

            if (this._justChecked)
            {
                this._justChecked = false;
                if (this.Selected)
                {
                    this._eventAggregator.PublishOnUIThread(new ItemMessage() { Item = this._item });
                }

                return;
            }

            this.Selected = false;
            this._eventAggregator.PublishOnUIThread(new ItemMessage() { Item = this._item, Delete = true });
        }

        /// <summary>
        /// Called when [checked].
        /// </summary>
        public void OnChecked()
        {
            this._justChecked = true;
        }

        /// <summary>
        /// Sets the selectable.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetSelectable(bool value)
        {
            this.Selectable = value;
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns>The default image Uri.</returns>
        private Uri GetDefaultImage()
        {
            switch (this._item.ItemClass)
            {
                case Patreon.Models.ItemClass.Amulet:
                case Patreon.Models.ItemClass.Ring:
                case Patreon.Models.ItemClass.Belt:
                case Patreon.Models.ItemClass.Quiver:
                    return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/4/45/Accessory_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074925");
                case Patreon.Models.ItemClass.BodyArmour:
                case Patreon.Models.ItemClass.Boots:
                case Patreon.Models.ItemClass.Gloves:
                case Patreon.Models.ItemClass.Helmet:
                case Patreon.Models.ItemClass.Shield:
                    return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/7/72/Armour_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074830");
                case Patreon.Models.ItemClass.Axe:
                case Patreon.Models.ItemClass.Bow:
                case Patreon.Models.ItemClass.Dagger:
                case Patreon.Models.ItemClass.Mace:
                case Patreon.Models.ItemClass.Sceptre:
                case Patreon.Models.ItemClass.Staff:
                case Patreon.Models.ItemClass.Sword:
                case Patreon.Models.ItemClass.Wand:
                    return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/9/90/Weapon_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074907");
                case Patreon.Models.ItemClass.Flask:
                    return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/a/a5/Flask_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074845");
                case Patreon.Models.ItemClass.Jewel:
                    return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/2/26/Jewel_item_icon.png/revision/latest/scale-to-width-down/80?cb=20150427211503");
            }

            return default;
        }

        #endregion
    }
}