using Client;
using System;
using System.Threading.Tasks;

// Add code here to send requests
Task one = ClientBehaviours.SendRequest("Hello there!", "TaskOne");
Task two = ClientBehaviours.SendRequest("Hello here!", "TaskTwo");
Task three = ClientBehaviours.SendRequest("Hello nowhere!", "TaskThree");

one.Wait();
two.Wait();
three.Wait();

Console.WriteLine("Execution finished");
Console.ReadLine();