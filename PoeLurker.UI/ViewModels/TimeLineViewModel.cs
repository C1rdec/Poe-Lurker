//-----------------------------------------------------------------------
// <copyright file="TimelineViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

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
    private readonly TimelineInformation _information;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineViewModel"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="maximum">The maximum.</param>
    public TimelineViewModel(double value, double maximum)
    {
        _information = new TimelineInformation()
        {
            MaximumValue = maximum,
            CurrentValue = value,
        };

        Items = new ObservableCollection<TimelineItemViewModel>();
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
    public double Value => _information.CurrentValue;

    /// <summary>
    /// Gets the maximum.
    /// </summary>
    public double Maximum => _information.MaximumValue;

    /// <summary>
    /// Gets the progress.
    /// </summary>
    public double Progress => _information.GetRelativePosition();

    #endregion

    #region Methods

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        Items.Clear();
    }

    /// <summary>
    /// Sets the progess.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetProgess(double value)
    {
        _information.CurrentValue = value;
        foreach (var item in Items)
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

        NotifyOfPropertyChange("Progress");
    }

    /// <summary>
    /// Sets the maximum value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetMaxValue(double value)
    {
        _information.MaximumValue = value;
        NotifyOfPropertyChange("Maximum");
        NotifyOfPropertyChange("Progress");
    }

    /// <summary>
    /// Adds the item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void AddItem(TimelineItemViewModel item)
    {
        item.SetInformation(_information);
        Items.Add(item);

        var maxValue = Items.Max(i => i.Value);
        if (Maximum != maxValue)
        {
            SetMaxValue(maxValue + 1);
        }
    }

    /// <summary>
    /// Called when an attached view's Loaded event fires.
    /// </summary>
    /// <param name="view">The view.</param>
    protected override void OnViewLoaded(object view)
    {
        _view = view as TimelineView;
        SetActualWidth();
        _view.SizeChanged += View_SizeChanged;
    }

    /// <summary>
    /// Handles the SizeChanged event of the view control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
    private void View_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        SetActualWidth();
    }

    /// <summary>
    /// Sets the actual width.
    /// </summary>
    private void SetActualWidth()
    {
        _information.ActualWidth = _view.ActualWidth;
        _actualWidth = _view.ActualWidth;
        NotifyOfPropertyChange("Progress");

        foreach (var item in Items)
        {
            item.NotifyLineChange();
        }
    }

    #endregion
}