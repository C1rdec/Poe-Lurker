//-----------------------------------------------------------------------
// <copyright file="TimelineItemViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Windows;
using Caliburn.Micro;
using PoeLurker.UI.Models;
using PoeLurker.UI.Services;

/// <summary>
/// Represent the time line.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class TimelineItemViewModel : PropertyChangedBase
{
    #region Fields

    private readonly double _value;
    private TimelineInformation _lineInformation;
    private bool _visited;
    private bool _reached;
    private PropertyChangedBase _detailedView;
    private bool _isOpen;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineItemViewModel" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public TimelineItemViewModel(double value)
    {
        _value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the value.
    /// </summary>
    public double Value => _value;

    /// <summary>
    /// Gets or sets the detailed view.
    /// </summary>
    public PropertyChangedBase DetailedView
    {
        get
        {
            return _detailedView;
        }

        set
        {
            _detailedView = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is open.
    /// </summary>
    public bool IsOpen
    {
        get
        {
            return _isOpen;
        }

        set
        {
            _isOpen = value;
            NotifyOfPropertyChange();
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
            return _visited;
        }

        set
        {
            _visited = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="TimelineItemViewModel"/> is reached.
    /// </summary>
    public bool Reached
    {
        get
        {
            return _reached;
        }

        set
        {
            _reached = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the margin.
    /// </summary>
    public Thickness Margin => new Thickness(_lineInformation.GetRelativePosition(_value) - 8, -3, 0, 0);

    #endregion

    #region Methods

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
    }

    /// <summary>
    /// Sets the information.
    /// </summary>
    /// <param name="lineInformation">The line information.</param>
    public void SetInformation(TimelineInformation lineInformation)
    {
        _lineInformation = lineInformation;

        if (_value <= lineInformation.CurrentValue)
        {
            Visited = true;
            Reached = true;
        }
    }

    /// <summary>
    /// Sets the width of the line.
    /// </summary>
    public void NotifyLineChange()
    {
        NotifyOfPropertyChange("Margin");
    }

    /// <summary>
    /// Opens this instance.
    /// </summary>
    public void Toggle()
    {
        if (Reached)
        {
            Visited = true;
        }

        if (IsOpen)
        {
            TimelineController.Close();
        }
        else
        {
            TimelineController.Open(this);
        }
    }

    #endregion
}