import 'dart:convert';

import 'package:poe_lurker_mobile/models/price.dart';

class Item {
  final String name;
  final String itemClass;
  final String buyerName;
  final String date;
  final String whisperMessage;
  final Price price;

  Item.fromJson(Map<dynamic, dynamic> json)
      : name = json['ItemName'],
        itemClass = json['ItemClass'],
        buyerName = json['PlayerName'],
        whisperMessage = json['WhisperMessage'],
        date = json['Date'],
         price = Price.fromJson(jsonDecode(json['Price']));
}