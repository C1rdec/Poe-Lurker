﻿//-----------------------------------------------------------------------
// <copyright file="SkillViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Lurker.Models;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents a skill viewmodel.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class SkillViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Skill _skill;
        private bool _selected;
        private bool _selectable;
        private bool _justChecked;
        private IEventAggregator _eventAggregator;

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
            this._skill = skill;
            this._selectable = selectable;
            this._eventAggregator = IoC.Get<IEventAggregator>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skill.
        /// </summary>
        public Skill Skill => this._skill;

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
        /// Gets the gems.
        /// </summary>
        public IEnumerable<GemViewModel> Gems => this._skill.Gems.OrderBy(g => g.Support).Select(g => new GemViewModel(g));

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        public void OnClick()
        {
            if (this._justChecked)
            {
                this._justChecked = false;
                if (this.Selected)
                {
                    this._eventAggregator.PublishOnUIThread(new SkillMessage() { Skill = this._skill });
                }

                return;
            }

            this.Selected = false;
            this._eventAggregator.PublishOnUIThread(new SkillMessage() { Skill = this._skill, Delete = true });
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

        #endregion
    }
}