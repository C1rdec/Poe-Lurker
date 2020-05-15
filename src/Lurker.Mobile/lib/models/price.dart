class Price {
  final String numberOfCurrencies;
  final String currencyType;

  Price.fromJson(Map<dynamic, dynamic> json)
      : numberOfCurrencies = json['NumberOfCurrencies'],
        currencyType = json['CurrencyType'];

  String toString(){
    return "${this.numberOfCurrencies} ${this.currencyType}";
  }
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