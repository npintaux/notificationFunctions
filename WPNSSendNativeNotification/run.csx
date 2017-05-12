#r "Microsoft.Azure.NotificationHubs"
#r "Newtonsoft.Json"

using System;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IAsyncCollector<Notification> notification, TraceWriter log)
{    
    log.Info($"Sending WPNS notification to all registered users - no tag, no template");    
    
    // In this example the queue item is a new user to be processed in the form of a JSON string with 
    // a "name" value.
    //
    // The XML format for a native WNS toast notification is ...
    // <?xml version="1.0" encoding="utf-8"?>
    // <toast>
    //      <visual>
    //     <binding template="ToastText01">
    //       <text id="1">notification message</text>
    //     </binding>
    //   </visual>
    // </toast>

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

    // construct the WPNS Payload and trigger the notification
    string wnsNotificationPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<toast><visual><binding template=\"ToastText01\">" +
                                        "<text id=\"1\">" + 
                                            "Message: " + msg.ToString() + ", URL: " + url.ToString() + 
                                        "</text>" +
                                    "</binding></visual></toast>";

    // trigger the WPNS notification with the payload
    log.Info($"{wnsNotificationPayload}");
    await notification.AddAsync(new WindowsNotification(wnsNotificationPayload));     
    
    // Return OK
    return req.CreateResponse(HttpStatusCode.OK, wnsNotificationPayload);
}