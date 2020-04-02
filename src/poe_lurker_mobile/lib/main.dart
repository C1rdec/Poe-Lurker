// Copyright 2018 The Flutter team. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'package:flutter/material.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
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
          child: Text('Hello World'),
        ),
      ),
    );
  }
}