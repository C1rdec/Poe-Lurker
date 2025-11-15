//-----------------------------------------------------------------------
// <copyright file="PopupViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using ProcessLurker;
using Winook;

/// <summary>
/// Represent the popup.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.Screen" />
public class PopupViewModel : PoeOverlayBase
{
    #region Fields

    private const int MouseMargin = 40;
    private const int PopupMargin = 10;
    private readonly MouseLurker _mouseLurker;
    private PoeWindowInformation _windowInformation;
    private int _x;
    private int _y;
    private double _opacity;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupViewModel"/> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="mouseLurker">The mouse lurker.</param>
    public PopupViewModel(DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService, MouseLurker mouseLurker)
        : base(dockingHelper, processLurker, settingsService)
    {
        _mouseLurker = mouseLurker;
        _opacity = 1;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the content.
    /// </summary>
    public PropertyChangedBase PopupContent { get; private set; }

    /// <summary>
    /// Gets a value indicating whether [content visible].
    /// </summary>
    public bool ContentVisible => PopupContent != null;

    /// <summary>
    /// Gets the opacity.
    /// </summary>
    public double Opacity
    {
        get
        {
            return _opacity;
        }

        private set
        {
            if (_opacity != value)
            {
                _opacity = value;
                NotifyOfPropertyChange();
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Opens the specified content.
    /// </summary>
    /// <param name="content">The content.</param>
    public void Open(PropertyChangedBase content)
    {
        ClearContent();
        SetPosition();
        SetContent(content);
        _mouseLurker.MouseMove += MouseLurker_MouseMove;
    }

    /// <summary>
    /// Called when an attached view's Loaded event fires.
    /// </summary>
    /// <param name="view">The view.</param>
    protected override void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        View.SizeChanged += View_SizeChanged;
    }

    /// <summary>
    /// Sets the content.
    /// </summary>
    /// <param name="content">The view.</param>
    private void SetContent(PropertyChangedBase content)
    {
        PopupContent = content;
        NotifyOfPropertyChange(() => PopupContent);
        NotifyOfPropertyChange(() => ContentVisible);
    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    private void SetPosition()
    {
        Execute.OnUIThread(() =>
        {
            _x = _mouseLurker.X;
            _y = _mouseLurker.Y;

            var currentWidth = View.Width;
            var rightSide = currentWidth + _mouseLurker.X;
            if (rightSide > _windowInformation.Position.Right)
            {
                View.Left = ApplyScalingX(_mouseLurker.X - (rightSide - _windowInformation.Position.Right) - PopupMargin);
            }
            else
            {
                View.Left = ApplyScalingX(_mouseLurker.X);
            }

            View.SizeToContent = System.Windows.SizeToContent.Manual;

            // +10 is to make sure the mouse is not over the overlay
            View.Top = ApplyScalingY(_mouseLurker.Y - (View.Height / 2));
            View.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        });
    }

    /// <summary>
    /// Clears the content.
    /// </summary>
    private void ClearContent()
    {
        if (PopupContent != null && PopupContent is IDisposable disposable)
        {
            disposable.Dispose();
        }

        PopupContent = null;
        NotifyOfPropertyChange(() => PopupContent);
        _mouseLurker.MouseMove -= MouseLurker_MouseMove;
    }

    /// <summary>
    /// Handles the MouseMove event of the MouseLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseMessageEventArgs"/> instance containing the event data.</param>
    private void MouseLurker_MouseMove(object sender, MouseMessageEventArgs e)
    {

        var differenceX = Math.Abs(_x - e.X);
        var differenceY = Math.Abs(_y - e.Y);
        var hypothenuse = Math.Sqrt(Math.Pow(differenceX, 2) + Math.Pow(differenceY, 2));

        var difference = MouseMargin - hypothenuse;
        if (difference <= 0)
        {
            ClearContent();
        }
        else
        {
            Opacity = difference / MouseMargin;
        }
    }

    /// <summary>
    /// Handles the SizeChanged event of the View control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
    private void View_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        var rightSide = e.NewSize.Width + _x + 50;
        if (rightSide > _windowInformation.Position.Right)
        {
            View.Left = ApplyScalingX(_x - (rightSide - _windowInformation.Position.Right) - PopupMargin - 10);
        }
        else
        {
            View.Left = ApplyScalingX(_x + 60);
        }
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        _windowInformation = windowInformation;
    }

    #endregion
}