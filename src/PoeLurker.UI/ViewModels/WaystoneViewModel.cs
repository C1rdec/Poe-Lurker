using Caliburn.Micro;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Services;

namespace PoeLurker.UI.ViewModels;

public class WaystoneViewModel : PropertyChangedBase
{
    private Map _map;
    private List<Affix> _damageAffixes;

    public WaystoneViewModel(Map map)
    {
        _map = map;
        _damageAffixes = map.DamageAffixes.ToList();
    }

    public bool FireDamage => _damageAffixes.Any(d => d.Text.Equals(AffixService.FireDamageText));

    public bool ColdDamage => _damageAffixes.Any(d => d.Text.Equals(AffixService.ColdDamageText));

    public bool LightningDamage => _damageAffixes.Any(d => d.Text.Equals(AffixService.LightningDamageText));

    public bool ChaosDamage => _damageAffixes.Any(d => d.Text.Equals(AffixService.ChaosDamageText));

    public bool Damage => _damageAffixes.Any(d => d.Text.Equals(AffixService.DamageText));

    public bool Critical => _damageAffixes.Any(d => d.Text.Equals(AffixService.CriticalText));

    public bool Safe => !_damageAffixes.Any();

    public bool CritOrDamage => Damage || Critical;

    public bool NoElemental => !(Safe || FireDamage || ColdDamage || LightningDamage);

    public int PackSize => _map.PackSize;

    public int ItemRarity => _map.ItemRarity;
}
