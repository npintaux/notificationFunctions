#r "Microsoft.Azure.NotificationHubs"
#r "Newtonsoft.Json"

using System;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IAsyncCollector<Notification> notification, TraceWriter log)
{    
    log.Info($"Sending GCM notification to all registered users - no tag, no template");    
    
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();
    if (data == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, "This request requires a json structure including a message and URL");
    }

    // extract the data to display
    var json = (JObject)JsonConvert.DeserializeObject(data.ToString());
    var msg = json["messageText"];
    var url = json["urlString"];
    if (msg == null || url == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, "One of the two input parameters is missing. Either 'messageText' or 'urlString'");
    }

    // construct the GCM Payload and trigger the notification
    string gcmNotificationPayload = "{\"data\": {\"message\": \"" + msg.ToString() + ", url: " + url.ToString() + "\" }}";
    log.Info($"Notification Payload: {gcmNotificationPayload}");
    await notification.AddAsync(new GcmNotification(gcmNotificationPayload));        
    
    // Return OK
    return req.CreateResponse(HttpStatusCode.OK, gcmNotificationPayload);
}