using Caliburn.Micro;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Services;

namespace PoeLurker.UI.ViewModels;

public class WaystoneViewModel : PropertyChangedBase
{
    private Map _map;

    public WaystoneViewModel(Map map)
    {
        _map = map;
    }

    public bool FireDamage => _map.DamageAffixes.Any(d => d.Text.Equals(AffixService.FireDamageText));

    public bool ColdDamage => _map.DamageAffixes.Any(d => d.Text.Equals(AffixService.ColdDamageText));

    public bool LightningDamage => _map.DamageAffixes.Any(d => d.Text.Equals(AffixService.LightningDamageText));

    public bool Damage => _map.DamageAffixes.Any(d => d.Text.Equals(AffixService.DamageText));

    public bool Critical => _map.DamageAffixes.Any(d => d.Text.Equals(AffixService.CriticalText));

    public bool Safe => !_map.DamageAffixes.Any();

    public bool CritOrDamage => Damage || Critical;

    public bool NoElemental => !(Safe || FireDamage || ColdDamage || LightningDamage);
}
