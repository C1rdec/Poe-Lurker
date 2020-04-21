import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

admin.initializeApp();
const fcm = admin.messaging();
const token = "[Token]";

// Start writing Firebase Functions
// https://firebase.google.com/docs/functions/typescript

export const sendTradeMessage = functions.https.onRequest((request, response) => {
 response.send("Hello from Firebase!");
 return fcm.send({token: token, data: {test:"allo"}});
});
