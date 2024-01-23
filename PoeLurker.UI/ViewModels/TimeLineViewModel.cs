//-----------------------------------------------------------------------
// <copyright file="TimelineViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Caliburn.Micro;
    using PoeLurker.UI.Models;
    using PoeLurker.UI.Views;

    /// <summary>
    /// The time line view model.
    /// </summary>
    public class TimelineViewModel : Screen, IViewAware
    {
        #region Fields

        private TimelineView _view;
        private double _actualWidth;
        private TimelineInformation _information;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineViewModel"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maximum">The maximum.</param>
        public TimelineViewModel(double value, double maximum)
        {
            this._information = new TimelineInformation()
            {
                MaximumValue = maximum,
                CurrentValue = value,
            };

            this.Items = new ObservableCollection<TimelineItemViewModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<TimelineItemViewModel> Items { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value => this._information.CurrentValue;

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        public double Maximum => this._information.MaximumValue;

        /// <summary>
        /// Gets the progress.
        /// </summary>
        public double Progress => this._information.GetRelativePosition();

        #endregion

        #region Methods

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Sets the progess.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetProgess(double value)
        {
            this._information.CurrentValue = value;
            foreach (var item in this.Items)
            {
                if (item.Value > value)
                {
                    item.Reached = false;
                    item.Visited = false;
                }
                else
                {
                    item.Reached = true;
                }
            }

            this.NotifyOfPropertyChange("Progress");
        }

        /// <summary>
        /// Sets the maximum value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetMaxValue(double value)
        {
            this._information.MaximumValue = value;
            this.NotifyOfPropertyChange("Maximum");
            this.NotifyOfPropertyChange("Progress");
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(TimelineItemViewModel item)
        {
            item.SetInformation(this._information);
            this.Items.Add(item);

            var maxValue = this.Items.Max(i => i.Value);
            if (this.Maximum != maxValue)
            {
                this.SetMaxValue(maxValue + 1);
            }
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            this._view = view as TimelineView;
            this.SetActualWidth();
            this._view.SizeChanged += this.View_SizeChanged;
        }

        /// <summary>
        /// Handles the SizeChanged event of the view control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void View_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.SetActualWidth();
        }

        /// <summary>
        /// Sets the actual width.
        /// </summary>
        private void SetActualWidth()
        {
            this._information.ActualWidth = this._view.ActualWidth;
            this._actualWidth = this._view.ActualWidth;
            this.NotifyOfPropertyChange("Progress");

            foreach (var item in this.Items)
            {
                item.NotifyLineChange();
            }
        }

        #endregion
    }
}