﻿//-----------------------------------------------------------------------
// <copyright file="MyCommand.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace Lurker.UI.Models
{
    public class MyCommand<T> : ICommand where T : class
    {
        public Predicate<T> CanExecuteDelegate { get; set; }
        public Action<T> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate == null || CanExecuteDelegate((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null) ExecuteDelegate((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
