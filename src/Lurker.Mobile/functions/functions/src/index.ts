import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

admin.initializeApp();
const fcm = admin.messaging();
const token = "c_QIkJm668Q:APA91bEN_AqY47lBMaK-rN6e7fk8r7Ey_tkUyRux3SeMVmlrNEkmBB7r6ujil5h-llJCV9itr5KfsPQjIIkhuXAnj_SBdSV_20gGtZsgPd2u1OtU4VO16aEDZE8WxAkgt8HnoASoma6l";

// Start writing Firebase Functions
// https://firebase.google.com/docs/functions/typescript

export const sendTradeMessage = functions.https.onRequest((request, response) => {
 response.send("Hello from Firebase!!");
 return fcm.send({token: token, data: request.body});
});
