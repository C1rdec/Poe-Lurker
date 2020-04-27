// Copyright 2018 The Flutter team. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:poe_lurker_mobile/components/itemWidget.dart';
import 'models/item.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData( primarySwatch: Colors.indigo, brightness: Brightness.light),
      color: Colors.red,
      title: 'Welcome to Flutter',
      home: Scaffold(
        appBar: AppBar(
          title: Text('Poe Lurker'),
        ),
        body: MessageHandler(),
      ),
    );
  }
}

class MessageHandler extends StatefulWidget {
  @override
  MessageHandlerState createState() => MessageHandlerState();
}

class MessageHandlerState extends State<MessageHandler> {
  final FirebaseMessaging _fcm = FirebaseMessaging();
  String lastMessage = "Waiting for message";
  Item lastItem;

 @override
 void initState() {
    super.initState();
    this._fcm.configure(
      onMessage: (Map<String, dynamic> message) async {
        try
        {
          var newItem = Item.fromJson(message["data"]);
          setState(() {
            lastMessage = "item";
            lastItem = newItem;
          });
        }
        catch(e){
          print(e);
        }
      },
      onLaunch: (Map<String, dynamic> message) async {
        setState(() {
          lastMessage = "$message";
        });
      },
      onResume: (Map<String, dynamic> message) async {
        setState(() {
          lastMessage = "$message";
        });
      } 
    );
    getTokenAsync();
  }

  @override
  Widget build(BuildContext context) {
    if (this.lastItem == null){
      return Text(this.lastMessage);
    }
    else {
      return ItemWidget(poeItem: this.lastItem);
    }
  }

  void getTokenAsync() async {
    String token = await this._fcm.getToken();
    print(token);
  }
}