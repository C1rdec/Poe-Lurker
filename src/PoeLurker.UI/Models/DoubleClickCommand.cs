//-----------------------------------------------------------------------
// <copyright file="DoubleClickCommand.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Models;

using System;
using System.Windows.Input;

/// <summary>
/// Represents a double click command.
/// </summary>
/// <seealso cref="System.Windows.Input.ICommand" />
public class DoubleClickCommand : ICommand
{
    #region Fields

    private readonly Action _action;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleClickCommand"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public DoubleClickCommand(Action action)
    {
        _action = action;
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    #endregion

    #region Methods

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    /// <returns>
    ///   <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
    /// </returns>
    public bool CanExecute(object parameter)
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    public void Execute(object parameter)
    {
        _action?.Invoke();
    }

    #endregion
}