//-----------------------------------------------------------------------
// <copyright file="WikiItemBaseViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Diagnostics;
using System.Web;
using System.Windows.Input;
using Caliburn.Micro;
using PoeLurker.Core.Extensions;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;

/// <summary>
/// Represents a wiki item.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public abstract class WikiItemBaseViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly WikiItem _item;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WikiItemBaseViewModel"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public WikiItemBaseViewModel(WikiItem item)
    {
        _item = item;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the wiki URL.
    /// </summary>
    public string Name => _item.Name;

    /// <summary>
    /// Gets the level.
    /// </summary>
    public int Level => _item.Level;

    /// <summary>
    /// Gets the wiki URL.
    /// </summary>
    public Uri WikiUrl => _item.WikiUrl;

    /// <summary>
    /// Gets the image URL.
    /// </summary>
    public Uri ImageUrl => _item.ImageUrl ?? DefaultImage;

    /// <summary>
    /// Gets the default image.
    /// </summary>
    public abstract Uri DefaultImage { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Opens the wiki.
    /// </summary>
    public void OpenWiki()
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            var league = IoC.Get<SettingsService>().RecentLeagueName;
            if (_item is Gem || string.IsNullOrEmpty(league))
            {
                ProcessExtensions.OpenUrl(WikiUrl.ToString());

                return;
            }

            var baseQuery = $"{{ \"query\": {{ \"status\": {{ \"option\": \"online\" }}, \"name\": \"{Name}\" }}, \"sort\": {{ \"price\": \"asc\" }} }}";

            var encoded = HttpUtility.UrlEncode(baseQuery);
            var search = $"https://www.pathofexile.com/trade/search/{league}?q={encoded}";
            ProcessExtensions.OpenUrl(search);
        }
    }

    #endregion
}