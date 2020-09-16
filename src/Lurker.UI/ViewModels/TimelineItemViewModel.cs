//-----------------------------------------------------------------------
// <copyright file="TimelineItemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.ComponentModel;
    using System.Windows;
    using Caliburn.Micro;
    using Lurker.UI.Models;

    /// <summary>
    /// Represent the time line.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class TimelineItemViewModel : PropertyChangedBase
    {
        #region Fields

        private double _value;
        private TimelineInformation _lineInformation;
        private bool _visited;
        private bool _reached;
        private INotifyPropertyChanged _detailedView;
        private bool _isOpen;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineItemViewModel" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public TimelineItemViewModel(double value)
        {
            this._value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value => this._value;

        /// <summary>
        /// Gets or sets the detailed view.
        /// </summary>
        public INotifyPropertyChanged DetailedView
        {
            get
            {
                return this._detailedView;
            }

            set
            {
                this._detailedView = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this._isOpen;
            }

            set
            {
                this._isOpen = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TimelineItemViewModel"/> is visited.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visited; otherwise, <c>false</c>.
        /// </value>
        public bool Visited
        {
            get
            {
                return this._visited;
            }

            set
            {
                this._visited = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TimelineItemViewModel"/> is reached.
        /// </summary>
        public bool Reached
        {
            get
            {
                return this._reached;
            }

            set
            {
                this._reached = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the margin.
        /// </summary>
        public Thickness Margin => new Thickness(this._lineInformation.GetRelativePosition(this._value) - 8, -3, 0, 0);

        #endregion

        #region Methods

        /// <summary>
        /// Sets the information.
        /// </summary>
        /// <param name="lineInformation">The line information.</param>
        public void SetInformation(TimelineInformation lineInformation)
        {
            this._lineInformation = lineInformation;

            if (this._value <= lineInformation.CurrentValue)
            {
                this.Visited = true;
                this.Reached = true;
            }
        }

        /// <summary>
        /// Sets the width of the line.
        /// </summary>
        public void NotifyLineChange()
        {
            this.NotifyOfPropertyChange("Margin");
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Toggle()
        {
            if (this.Reached)
            {
                this.Visited = true;
            }

            this.IsOpen = !this.IsOpen;
        }

        #endregion
    }
}