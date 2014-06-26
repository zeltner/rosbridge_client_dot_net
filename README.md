Rosbridge .NET Client
=====================

This is a .NET library which provides a client for the [Rosbridge Suite](http://wiki.ros.org/rosbridge_suite).

### Overview
The Visual Studio solution comes with three projects:
- rosbridge_client_common : The common part of the library
- rosbridge_client_socket_for_desktop : The WPF part of the library (for Windows desktop applications)
- rosbridge_client_example_wpf : Windows desktop applications example

### Current Status
Publishing and subscribing to a topic and calling a service are implemented:

```csharp
using Rosbridge.Client;


// Start the message dispatcher
var md = new MessageDispatcher(new Socket(new Uri("ws://localhost:9090")), new MessageSerializerV2_0());
await md.StartAsync();


// Subscribe to a topic 
var subscriber = new Subscriber("/my_topic", "std_msgs/String", md);
subscriber.MessageReceived += (s, e) =>
            {
                dynamic message = e.Message;
                
                // A std_msgs/String message looks like { "msg" : { "data" : <content> } }
                string content = message.msg.data;
            };
await subscriber.SubscribeAsync();


// Publish to a topic
var publisher = new Publisher("/my_topic", "std_msgs/String", md);
await publisher.AdvertiseAsync();
await publisher.PublishAsync(new { data = "Hello World" });


// Call a service
var serviceClient = new ServiceClient("/add_two_ints", md);
List<dynamic> args = new List<dynamic>();
args.Add(10);
args.Add(2);
var result = serviceClient.Call(args);

// Clean-up
await subscriber.UnsubscribeAsync();
await publisher.UnadvertiseAsync();
await md.StopAsync();
```
