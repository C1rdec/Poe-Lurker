//-----------------------------------------------------------------------
// <copyright file="BulbViewModelBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.UI.Models;
using ProcessLurker;

/// <summary>
/// BulbViewModel.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.PoeOverlayBase" />
public abstract class BulbViewModelBase : PoeOverlayBase
{
    #region Fields

    /// <summary>
    /// The default bulb height.
    /// </summary>
    protected static readonly int DefaultBulbHeight = 220;

    private INotifyPropertyChanged _actionView;
    private System.Action _stickyAction;
    private INotifyPropertyChanged _stickyActionView;
    private CancellationTokenSource _tokenSource;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BulbViewModelBase" /> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The Processs lurker.</param>
    /// <param name="settingsService">The settings serivce.</param>
    /// <param name="clientLurker">The client lurker.</param>
    public BulbViewModelBase(DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService, ClientLurker clientLurker)
        : base(dockingHelper, processLurker, settingsService)
    {
        ClientLurker = clientLurker;
        SetDefaultAction();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this instance has action.
    /// </summary>
    public bool HasAction => Action != null;

    /// <summary>
    /// Gets a value indicating whether this instance is default action.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is default action; otherwise, <c>false</c>.
    /// </value>
    public bool IsDefaultAction => DefaultAction == Action;

    /// <summary>
    /// Gets or sets the action view.
    /// </summary>
    public INotifyPropertyChanged ActionView
    {
        get
        {
            return _actionView;
        }

        protected set
        {
            _actionView = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the client lurker.
    /// </summary>
    protected ClientLurker ClientLurker { get; private set; }

    /// <summary>
    /// Gets the default action.
    /// </summary>
    protected virtual System.Action DefaultAction => null;

    /// <summary>
    /// Gets or sets the action.
    /// </summary>
    protected System.Action Action { get; set; }

    /// <summary>
    /// Gets or sets the sub action.
    /// </summary>
    protected System.Action SubAction { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Called when [click].
    /// </summary>
    public void OnClick()
    {
        if (Action == null)
        {
            DefaultAction?.Invoke();
            return;
        }

        Action.Invoke();
    }

    /// <summary>
    /// Hides this instance.
    /// </summary>
    protected void Hide()
    {
        ActionView = null;
        SubAction = null;
        Action = _stickyAction;
        ActionView = _stickyActionView;
        _stickyAction = null;
        _stickyActionView = null;
        NotifyOfPropertyChange(nameof(HasAction));
    }

    /// <summary>
    /// Sets the default action.
    /// </summary>
    protected void SetDefaultAction()
    {
        if (DefaultAction == null)
        {
            return;
        }

        void firstLocationChanged(object s, LocationChangedEvent a)
        {
            ClientLurker.LocationChanged -= firstLocationChanged;
            SetAction(new BulbMessage() { Action = DefaultAction });
        }

        ClientLurker.LocationChanged += firstLocationChanged;
    }

    /// <summary>
    /// Sets the action.
    /// </summary>
    /// <param name="message">The message.</param>
    protected virtual void SetAction(BulbMessage message)
    {
        if (_tokenSource != null)
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;
        }

        message.OnShow?.Invoke(Hide);

        if (message.Sticky)
        {
            _stickyAction = Action;
            _stickyActionView = _actionView;
        }

        if (ActionView is IDisposable disposableView)
        {
            disposableView.Dispose();
        }

        Action = message.Action;
        SubAction = message.SubAction;
        ActionView = message.View;

        NotifyOfPropertyChange(nameof(HasAction));

        if (message.DisplayTime != TimeSpan.Zero)
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Run(async () => await Task.Delay(message.DisplayTime)).ContinueWith((t) =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                Hide();
            });
        }
    }

    #endregion
}