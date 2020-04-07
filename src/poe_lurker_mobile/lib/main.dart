// Copyright 2018 The Flutter team. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  final FirebaseMessaging _fcm = FirebaseMessaging();

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData( primarySwatch: Colors.indigo,  brightness: Brightness.light),
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

  Future<String> getTokenAsync() async {
    return this._fcm.getToken();
  }
}

class MessageHandler extends StatefulWidget {
  @override
  MessageHandlerState createState() => MessageHandlerState();
}

class MessageHandlerState extends State<MessageHandler> {
  final FirebaseMessaging _fcm = FirebaseMessaging();
  String token = "Loading";

 @override
 void initState() {
    super.initState();
    this._fcm.configure(
      onMessage: (Map<String, dynamic> message) async {
        print("onMessage: $message");
      },
      onLaunch: (Map<String, dynamic> message) async {
        print("onLaunch: $message");
      },
      onResume: (Map<String, dynamic> message) async {
        print("onResume: $message");
      } 
    );
    getTokenAsync();
  }

  @override
  Widget build(BuildContext context) {
    return Text(this.token);
  }

  void getTokenAsync() async {
    var test = await this._fcm.getToken();
    setState(() {
      token = test;
    });
  }
}