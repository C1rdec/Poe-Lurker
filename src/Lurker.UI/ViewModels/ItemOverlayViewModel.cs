//-----------------------------------------------------------------------
// <copyright file="ItemOverlayViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represents the ItemOverlay.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class ItemOverlayViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private static readonly int MaxAffixCount = 3;
        private PoeItem _item;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemOverlayViewModel" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ItemOverlayViewModel(PoeItem item)
        {
            this._item = item;
            this.SocketInformation = GetSocketInformation(item);
            this.Suffixes = new List<char>();
            this.Prefixes = new List<char>();

            if (this._item.Information != null)
            {
                var openPrefix = MaxAffixCount - this._item.Information.PrefixCount;
                for (int i = 0; i < openPrefix; i++)
                {
                    this.Prefixes.Add('.');
                }

                var openSuffix = MaxAffixCount - this._item.Information.SuffixCount;
                for (int i = 0; i < openSuffix; i++)
                {
                    this.Suffixes.Add('.');
                }
            }
        }

        #endregion

        #region Properties

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
        public ItemClass ItemClass => this._item.ItemClass;

        /// <summary>
        /// Gets the itemlevel.
        /// </summary>
        public int ItemLevel => this._item.ItemLevel;

        /// <summary>
        /// Gets the rarity.
        /// </summary>
        public Rarity Rarity => this._item.Rarity;

        /// <summary>
        /// Gets the type of the base.
        /// </summary>
        public string BaseType => this._item.BaseType;

        /// <summary>
        /// Gets the item information.
        /// </summary>
        public string ItemInformation => $"({this.ItemLevel}) {this.BaseType} {this.ItemClass}";

        /// <summary>
        /// Gets the socket information.
        /// </summary>
        public string SocketInformation { get; private set; }

        /// <summary>
        /// Gets the total life count.
        /// </summary>
        public double TotalLifeCount => this._item.TotalLifeCount;

        /// <summary>
        /// Gets the total cold resistance.
        /// </summary>
        public double TotalColdResistance => this._item.TotalColdResistance;

        /// <summary>
        /// Gets the total fire resistance.
        /// </summary>
        public double TotalFireResistance => this._item.TotalFireResistance;

        /// <summary>
        /// Gets the total lightning resistance.
        /// </summary>
        public double TotalLightningResistance => this._item.TotalLightningResistance;

        /// <summary>
        /// Gets the total elemental resistance.
        /// </summary>
        public double TotalElementalResistance => this._item.TotalElementalResistance;

        /// <summary>
        /// Gets the important affixed.
        /// </summary>
        public IEnumerable<AffixViewModel> ImportantAffixes => this._item.ImportantAffixes.Where(i => i != null).Select(i => new AffixViewModel(i));

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
}