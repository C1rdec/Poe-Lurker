class Price {
  final String numberOfCurrencies;
  final String currencyType;

  Price.fromJson(Map<dynamic, dynamic> json)
      : numberOfCurrencies = json['NumberOfCurrencies'],
        currencyType = json['CurrencyType'];
}

enum CurrencyType { 
    Unknown,
    Mirror,
    Exalted,
    Chaos,
    Fusing,
    Alchemy,
    Vaal,
    Chromatic,
    Regal,
    Divine,
    Alteration,
    Jeweller,
    Gemcutter
}