using Caliburn.Micro;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Services;

namespace PoeLurker.UI.ViewModels;

public class WaystoneViewModel : PropertyChangedBase
{
    private Map _map;
    private List<Affix> _damageAffixes;
    private List<Affix> _dangerousAffixes;
    private List<Affix> _curseAffixes;

    public WaystoneViewModel(Map map)
    {
        _map = map;
        _damageAffixes = map.DamageAffixes.ToList();
        _dangerousAffixes = map.DangerousAffixes.ToList();
        _curseAffixes = map.CurseAffixes.ToList();
    }

    public bool TemporalChain => _curseAffixes.Any(d => d.Text.Contains("Temporal Chains"));

    public bool ElementalWeakness => _curseAffixes.Any(d => d.Text.Contains("Elemental Weakness"));

    public bool Vulnerability => _curseAffixes.Any(d => d.Text.Contains("Vulnerability"));

    public bool Enfeeble => _curseAffixes.Any(d => d.Text.Contains("Enfeeble"));

    public bool ReflectElemental => _dangerousAffixes.Any(d => d.Text.Contains("Elemental Damage"));

    public bool ReflectPhysical => _dangerousAffixes.Any(d => d.Text.Contains("Physical Damage"));

    public bool BothReflect => ReflectElemental && ReflectPhysical;

    public bool FireDamage => _damageAffixes.Any(d => d.Text.Contains("Fire"));

    public bool ColdDamage => _damageAffixes.Any(d => d.Text.Contains("Cold"));

    public bool LightningDamage => _damageAffixes.Any(d => d.Text.Contains("Lightning"));

    public bool ChaosDamage => _damageAffixes.Any(d => d.Text.Contains("Chaos"));

    public bool Damage => _damageAffixes.Any(d => d.Text.Equals(AffixService.DamageText));

    public bool Critical => _damageAffixes.Any(d => d.Text.Contains("Critical"));

    public bool Safe => !_damageAffixes.Any();

    public bool CritOrDamage => Damage || Critical;

    public bool NoElemental => !(Safe || FireDamage || ColdDamage || LightningDamage || ChaosDamage);

    public bool HasCurse => _curseAffixes.Any();

    public int PackSize => _map.PackSize;

    public int ItemRarity => _map.ItemRarity;

    public int ItemQuantity => _map.ItemQuantity;

    public bool HasQuantity => _map.ItemQuantity > 0;
}
