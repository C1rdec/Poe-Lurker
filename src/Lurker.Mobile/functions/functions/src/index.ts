import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

admin.initializeApp();
const fcm = admin.messaging();
const token = "fvLvwkY1BZU:APA91bGl2BGBEHSYusmP9RD8B5x5hYRUNDLX3cwUTO1GFRcXXsiYLH5SaHVNgbN0uknVSFGkzv2wGAJ0MBY0XZUzN2NzZpRgOslG2kKCd38-8wEPyPl_rqCQvzqt3PDmTQcJ__YBFwy_";

// Start writing Firebase Functions
// https://firebase.google.com/docs/functions/typescript

export const sendTradeMessage = functions.https.onRequest((request, response) => {
 response.send(request.body);
 const item = JSON.parse(request.body.Item);
 return fcm.send({token: token, data: item});
});
