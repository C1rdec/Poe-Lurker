//-----------------------------------------------------------------------
// <copyright file="ItemOverlayViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Collections.Generic;
using System.Linq;
using PoeLurker.Patreon.Models;

/// <summary>
/// Represents the ItemOverlay.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class ItemOverlayViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private static readonly int MaxAffixCount = 3;
    private readonly PoeItem _item;
    private readonly AffixViewModel _importantAffix;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemOverlayViewModel" /> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public ItemOverlayViewModel(PoeItem item)
    {
        _item = item;
        SocketInformation = GetSocketInformation(item);
        Suffixes = new List<char>();
        Prefixes = new List<char>();

        if (_item.Information != null)
        {
            var openPrefix = MaxAffixCount - _item.Information.PrefixCount;
            for (var i = 0; i < openPrefix; i++)
            {
                Prefixes.Add('.');
            }

            var openSuffix = MaxAffixCount - _item.Information.SuffixCount;
            for (var i = 0; i < openSuffix; i++)
            {
                Suffixes.Add('.');
            }
        }

        var affix = _item.ImportantAffixes.FirstOrDefault(i => i != null);
        if (affix != null)
        {
            _importantAffix = new AffixViewModel(affix);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the item is good.
    /// </summary>
    public bool IsGood => _item.IsGood();

    /// <summary>
    /// Gets the suffix count.
    /// </summary>
    public List<char> Suffixes { get; }

    /// <summary>
    /// Gets the prefix count.
    /// </summary>
    public List<char> Prefixes { get; }

    /// <summary>
    /// Gets the item class.
    /// </summary>
    public ItemClass ItemClass => _item.ItemClass;

    /// <summary>
    /// Gets the itemlevel.
    /// </summary>
    public int ItemLevel => _item.ItemLevel;

    /// <summary>
    /// Gets the rarity.
    /// </summary>
    public Rarity Rarity => _item.Rarity;

    /// <summary>
    /// Gets the type of the base.
    /// </summary>
    public string BaseType => _item.BaseType;

    /// <summary>
    /// Gets the item information.
    /// </summary>
    public string ItemInformation => $"({ItemLevel}) {BaseType} {ItemClass}";

    /// <summary>
    /// Gets the socket information.
    /// </summary>
    public string SocketInformation { get; private set; }

    /// <summary>
    /// Gets the total life count.
    /// </summary>
    public double TotalLifeCount => _item.TotalLifeCount;

    /// <summary>
    /// Gets the total cold resistance.
    /// </summary>
    public double TotalColdResistance => _item.TotalColdResistance;

    /// <summary>
    /// Gets the total fire resistance.
    /// </summary>
    public double TotalFireResistance => _item.TotalFireResistance;

    /// <summary>
    /// Gets the total lightning resistance.
    /// </summary>
    public double TotalLightningResistance => _item.TotalLightningResistance;

    /// <summary>
    /// Gets the total elemental resistance.
    /// </summary>
    public double TotalElementalResistance => _item.TotalElementalResistance;

    /// <summary>
    /// Gets the important affixed.
    /// </summary>
    public IEnumerable<AffixViewModel> ImportantAffixes => _item.ImportantAffixes.Where(i => i != null).Select(i => new AffixViewModel(i));

    /// <summary>
    /// Gets the important affixes.
    /// </summary>
    public AffixViewModel FirstImportantAffix => _importantAffix;

    /// <summary>
    /// Gets a value indicating whether this instance has important affix.
    /// </summary>
    public bool HasImportantAffix => _importantAffix != null;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the socket information.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The socket information.</returns>
    private static string GetSocketInformation(PoeItem item)
    {
        if (item is SocketableItem socketItem)
        {
            return $"{socketItem.SocketCount} sockets | {socketItem.SocketLinks} links";
        }

        return string.Empty;
    }

    #endregion
}