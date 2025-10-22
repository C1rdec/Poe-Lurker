using Caliburn.Micro;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Services;

namespace PoeLurker.UI.ViewModels;

public class WaystoneViewModel : PropertyChangedBase
{
    private Map _map;
    private List<Affix> _damageAffixes;
    private List<Affix> _dangerousAffixes;

    public WaystoneViewModel(Map map)
    {
        _map = map;
        _damageAffixes = map.DamageAffixes.ToList();
        _dangerousAffixes = map.DangerousAffixes.ToList();
    }

    public bool ReflectPhysical => _dangerousAffixes.Any(d => d.Text.Contains("Physical Damage"));

    public bool ReflectElemental => _dangerousAffixes.Any(d => d.Text.Contains("Elemental Damage"));

    public bool FireDamage => _damageAffixes.Any(d => d.Text.Contains("Fire"));

    public bool ColdDamage => _damageAffixes.Any(d => d.Text.Contains("Cold"));

    public bool LightningDamage => _damageAffixes.Any(d => d.Text.Contains("Lightning"));

    public bool ChaosDamage => _damageAffixes.Any(d => d.Text.Contains("Chaos"));

    public bool Damage => _damageAffixes.Any(d => d.Text.Equals(AffixService.DamageText));

    public bool Critical => _damageAffixes.Any(d => d.Text.Contains("Critical"));

    public bool Safe => !_damageAffixes.Any();

    public bool CritOrDamage => Damage || Critical;

    public bool NoElemental => !(Safe || FireDamage || ColdDamage || LightningDamage || ChaosDamage);

    public int PackSize => _map.PackSize;

    public int ItemRarity => _map.ItemRarity;

    public int ItemQuantity => _map.ItemQuantity;

    public bool HasQuantity => _map.ItemQuantity > 0;
}
