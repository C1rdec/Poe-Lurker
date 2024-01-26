//-----------------------------------------------------------------------
// <copyright file="UniqueItemView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views;

using System.Windows.Controls;

/// <summary>
/// Interaction logic for UniqueItemView.xaml.
/// </summary>
public partial class UniqueItemView : UserControl
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueItemView"/> class.
    /// </summary>
    public UniqueItemView()
    {
        InitializeComponent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the MouseEnter event of the Grid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
    private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Popup.IsOpen = true;
    }

    /// <summary>
    /// Handles the MouseLeave event of the Grid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
    private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Popup.IsOpen = false;
    }

    #endregion
}