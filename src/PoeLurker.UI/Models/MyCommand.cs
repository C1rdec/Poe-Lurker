﻿namespace PoeLurker.UI.Models;

using System;
using System.Windows.Input;

/// <summary>
/// Represents MyCommand for the charts.
/// </summary>
/// <typeparam name="T">Generics.</typeparam>
/// <seealso cref="System.Windows.Input.ICommand" />
public class MyCommand<T> : ICommand
    where T : class
{
    #region Properties

    /// <summary>
    /// Gets or sets the can execute delegate.
    /// </summary>
    public Predicate<T> CanExecuteDelegate { get; set; }

    /// <summary>
    /// Gets or sets the execute delegate.
    /// </summary>
    public Action<T> ExecuteDelegate { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether this instance can execute the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>
    ///   <c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool CanExecute(object parameter)
    {
        return CanExecuteDelegate == null || CanExecuteDelegate((T)parameter);
    }

    /// <summary>
    /// Executes the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public void Execute(object parameter)
    {
        if (ExecuteDelegate != null)
        {
            ExecuteDelegate((T)parameter);
        }
    }

    /// <summary>
    /// Occurs when [can execute changed].
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    #endregion
}