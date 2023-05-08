// See https://aka.ms/new-console-template for more information
using MQTTnet.Client;
using MQTTnet;
using MQTTNet_Console.Helpers;
using System.Runtime.CompilerServices;

MQTTNet_Console.Helpers.MQTTNetHelper _mqTTHelper = new MQTTNetHelper();

await _mqTTHelper.Publish_Application_Message();

await _mqTTHelper.Handle_Received_Application_Message();