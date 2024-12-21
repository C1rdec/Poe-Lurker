//-----------------------------------------------------------------------
// <copyright file="MapAffixViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Linq;
using PoeLurker.Patreon.Services;

/// <summary>
/// `Represents a dangerous map affix.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class MapAffixViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    /// <summary>
    /// The reflect physical identifier.
    /// </summary>
    public const string ReflectPhysicalId = "explicit.stat_3464419871";

    /// <summary>
    /// The reflect elemental identifier.
    /// </summary>
    public const string ReflectElementalId = "explicit.stat_2764017512";

    /// <summary>
    /// The cannot regenerate identifier.
    /// </summary>
    public const string CannotRegenerateId = "explicit.stat_1910157106";

    /// <summary>
    /// The cannot leech identifier.
    /// </summary>
    public const string CannotLeechId = "explicit.stat_1140978125";

    /// <summary>
    /// The temporal chains identifier.
    /// </summary>
    public const string TemporalChainsId = "explicit.stat_2326202293";

    /// <summary>
    /// The avoid ailments identifier.
    /// </summary>
    public const string AvoidAilmentsId = "explicit.stat_322206271";

    public const string AvoidAilmentsId2 = "explicit.stat_1994551050";

    private readonly string _id;
    private readonly PlayerViewModel _playerViewModel;
    private readonly bool _reflectPhysical;
    private readonly bool _reflectElemental;
    private readonly bool _cannotRegenerate;
    private readonly bool _cannotLeech;
    private readonly bool _temporalChains;
    private readonly bool _avoidAilments;
    private bool _helpVisible;
    private readonly bool _ignored;
    private bool _selected;
    private readonly string _name;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MapAffixViewModel" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="selectable">if set to <c>true</c> [selectable].</param>
    /// <param name="playerViewModel">The player view model.</param>
    /// <param name="ignored">if set to <c>true</c> [ignored].</param>
    public MapAffixViewModel(string id, bool selectable, PlayerViewModel playerViewModel, bool ignored = false)
    {
        Selectable = selectable;
        switch (id)
        {
            case ReflectPhysicalId:
                _reflectPhysical = true;
                break;
            case ReflectElementalId:
                _reflectElemental = true;
                break;
            case CannotRegenerateId:
                _cannotRegenerate = true;
                break;
            case CannotLeechId:
                _cannotLeech = true;
                break;
            case TemporalChainsId:
                _temporalChains = true;
                break;
            case AvoidAilmentsId:
            case AvoidAilmentsId2:
                _avoidAilments = true;
                break;
        }

        _id = id;
        _playerViewModel = playerViewModel;
        _selected = !_playerViewModel.IgnoredMadMods.Contains(id);
        _name = AffixService.GetAffixText(id);
        _ignored = ignored;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="MapAffixViewModel"/> is selected.
    /// </summary>
    public bool Selected
    {
        get
        {
            return _selected;
        }

        private set
        {
            _selected = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether [popup visible].
    /// </summary>
    public bool HelpVisible
    {
        get
        {
            return _helpVisible;
        }

        private set
        {
            _helpVisible = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="MapAffixViewModel"/> is ignored.
    /// </summary>
    public bool Ignored => _ignored;

    /// <summary>
    /// Gets a value indicating whether [reflect physical].
    /// </summary>
    public bool ReflectPhysical => _reflectPhysical;

    /// <summary>
    /// Gets a value indicating whether [reflect elemental].
    /// </summary>
    public bool ReflectElemental => _reflectElemental;

    /// <summary>
    /// Gets a value indicating whether [cannot regenerate].
    /// </summary>
    public bool CannotRegenerate => _cannotRegenerate;

    /// <summary>
    /// Gets a value indicating whether [cannot leech].
    /// </summary>
    public bool CannotLeech => _cannotLeech;

    /// <summary>
    /// Gets a value indicating whether [temporal chains].
    /// </summary>
    public bool TemporalChains => _temporalChains;

    /// <summary>
    /// Gets a value indicating whether [avoid ailments].
    /// </summary>
    public bool AvoidAilments => _avoidAilments;

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets a value indicating whether this <see cref="MapAffixViewModel"/> is selectable.
    /// </summary>
    public bool Selectable { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Called when [click].
    /// </summary>
    public void OnClick()
    {
        if (!Selectable)
        {
            return;
        }

        if (Selected)
        {
            _playerViewModel.AddIgnoredMapMod(_id);
        }
        else
        {
            _playerViewModel.RemoveIgnoredMapMod(_id);
        }

        Selected = !Selected;
    }

    /// <summary>
    /// Mouses the enter.
    /// </summary>
    public void MouseEnter()
    {
        HelpVisible = true;
    }

    /// <summary>
    /// Mouses the leave.
    /// </summary>
    public void MouseLeave()
    {
        HelpVisible = false;
    }

    #endregion
}