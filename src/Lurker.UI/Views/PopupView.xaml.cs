//-----------------------------------------------------------------------
// <copyright file="PopupView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for PopupView.xaml.
    /// </summary>
    public partial class PopupView : Window
    {
        #region Fields

        private Window _parent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupView"/> class.
        /// </summary>
        public PopupView()
        {
           this.InitializeComponent();
           this.HideFromAltTab();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hides the window from alt tab.
        /// </summary>
        private void HideFromAltTab()
        {
            this._parent = new Window
            {
                Top = -100,
                Left = -100,
                Width = 1,
                Height = 1,
                WindowStyle = WindowStyle.ToolWindow, // Set window style as ToolWindow to avoid its icon in AltTab
                ShowInTaskbar = false,
            };

            this._parent.Show();
            this.Owner = this._parent;
            this._parent.Hide();
        }

        #endregion
    }
}