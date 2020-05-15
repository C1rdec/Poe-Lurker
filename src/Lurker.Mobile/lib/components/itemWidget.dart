import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:poe_lurker_mobile/models/item.dart';

class ItemWidget extends StatelessWidget {
  final Item poeItem;

  ItemWidget({ this.poeItem });

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: <Widget>[
          ListTile(
            leading: Icon(Icons.comment),
            title: Text(this.poeItem.name),
            subtitle: Text(this.poeItem.price.toString()),
          )
        ], 
      ),
    );
  }
} 