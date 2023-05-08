using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MQTTNet_Console.Models;

namespace MQTTNet_Console.Helpers
{
    public class MQTTNetHelper
    {
        private readonly string _broker = "7e4b48c3e103405ea875e85951a775c3.s2.eu.hivemq.cloud";

        public async Task Handle_Received_Application_Message()
        {
            /*
             * This sample subscribes to a topic and processes the received message.
             */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_broker)
                    .WithTls()
                    .WithCredentials("HiveMQ_Client", "BonJas-141")
                    .Build();

                using (var timeout = new CancellationTokenSource(5000))
                {
                    await mqttClient.ConnectAsync(mqttClientOptions, timeout.Token);
                }

                // Setup message handling before connecting so that queued messages
                // are also handled properly. When there is no event handler attached all
                // received messages get lost.
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    Console.WriteLine("Received application message.");
                    Console.WriteLine();
                    string message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                    Telemetry telemetry = JsonSerializer.Deserialize<Telemetry>(message);
                    Console.WriteLine($"Your received message is: {message}");
                    Console.WriteLine();
                    Console.WriteLine("After deserializing your incoming message:");
                    Console.WriteLine($"Temperature: {telemetry.Temperature}"); 
                    Console.WriteLine($"Humidity: {telemetry.Humidity}");
                    Console.WriteLine($"Time: {telemetry.Time}");

                    Console.WriteLine("Press ESC to exit or wait for a message.");
                    Console.ReadLine();

                    return Task.CompletedTask;
                };

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic("home/temp");
                        })
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Console.WriteLine("MQTT client subscribed to topic.");

                Console.WriteLine("Press ESC to exit or wait for a message.");
                Console.ReadLine();
            }
        }

        public async Task Publish_Application_Message()
        {
            /*
             * This sample pushes a simple application message including a topic and a payload.
             *
             * Always use builders where they exist. Builders (in this project) are designed to be
             * backward compatible. Creating an _MqttApplicationMessage_ via its constructor is also
             * supported but the class might change often in future releases where the builder does not
             * or at least provides backward compatibility where possible.
             */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_broker)
                    .WithTls()
                    .WithCredentials("HiveMQ_Client", "BonJas-141")
                    .Build();

                using (var timeout = new CancellationTokenSource(5000))
                {
                    await mqttClient.ConnectAsync(mqttClientOptions, timeout.Token);
                }                

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("home/temp")
                    .WithPayload("19.5")
                    .Build();

                await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

                await mqttClient.DisconnectAsync();

                Console.WriteLine("MQTT application message is published.");
            }
        }
    }
}
