# notificationFunctions
A set of Azure Functions with a binding to NotificationHub to broadcast notifications to mobile users.

## Setup
The Azure Functions make use of a binding on NotificationHub. It is therefore necessary to update each function.json file to provide:
1. the NotificationHub service name
2. the Default connection string of the NotificationHub service

## Message formats

### GCMSendNativeNotification
This Azure function accepts a HTTP request with a JSON structure in the body. By default, the structure looks like this:

{
    "messageText": "Hello, world!",
    "urlString": "http://www.contoso.com"
}

