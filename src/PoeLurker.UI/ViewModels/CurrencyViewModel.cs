using PoeLurker.Core.Models;
using PoeLurker.Patreon.Models;

namespace PoeLurker.UI.ViewModels;

public class CurrencyViewModel : Caliburn.Micro.PropertyChangedBase
{
    private readonly CurrencyType _currencyType;

    public CurrencyViewModel(CurrencyType currencyType)
    {
        _currencyType = currencyType;
    }

    public string ImageSource => GetSource();

    private string GetSource()
    {
        return _currencyType switch
        {
            CurrencyType.Chaos => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Chaos.png" : "/Assets/Chaos.png",
            CurrencyType.Exalted => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Exalted.png" : "/Assets/Exalted.png",
            CurrencyType.Mirror => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Mirror.png" : "/Assets/Mirror.png",
            CurrencyType.Vaal => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Vaal.png" : "/Assets/Vaal.png",
            CurrencyType.Alchemy => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Alchemy.png" : "/Assets/Alchemy.png",
            CurrencyType.Regal => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Regal.png" : "/Assets/Regal.png",
            CurrencyType.Divine => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Divine.png" : "/Assets/Divine.png",
            CurrencyType.Chance => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Chance.png" : "/Assets/Chance.png",
            CurrencyType.Gemcutter => PoeApplicationContext.Poe2 ? "/Assets/Poe2/Gemcutter.png" : "/Assets/Gemcutter.png",
            CurrencyType.Chisel => "/Assets/Chisel.png",
            CurrencyType.Sextant => "/Assets/Sextant.png",
            CurrencyType.Fusing => "/Assets/Fusing.png",
            CurrencyType.Chromatic => "/Assets/Chromatic.png",
            CurrencyType.Alteration => "/Assets/Alteration.png",
            CurrencyType.Jeweller => "/Assets/Jeweller.png",
            CurrencyType.Scouring => "/Assets/Scouring.png",
            _ => string.Empty
        };
    }
}
