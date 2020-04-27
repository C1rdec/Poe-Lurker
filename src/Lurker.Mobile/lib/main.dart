// Copyright 2018 The Flutter team. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
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
        body: Center(
          child: MessageHandler(),
        ),
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

 @override
 void initState() {
    super.initState();
    this._fcm.configure(
      onMessage: (Map<String, dynamic> message) async {
        try
        {
          var item = Item.fromJson(message["data"]);
          print(item);
        }
        catch(e){
          print(e);
        }
        
        setState(() {
          lastMessage = "item";
        });
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
    return Text(this.lastMessage);
  }

  void getTokenAsync() async {
    String token = await this._fcm.getToken();
    print(token);
  }
}