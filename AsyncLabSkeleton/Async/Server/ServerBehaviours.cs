using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class ServerBehaviours
    {
        private static TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        // Create 10 threads (max number of concurrent requests)
        private static Thread[] serverThreads = new Thread[10];
        public static void StartServer()
        {
            tcpListener.Start();
            Console.WriteLine("[!] Server Active");

            // Start all of the threads
            for (int i = 0; i < 10; i++)
            {
                serverThreads[i] = new Thread(ServerThread);
                serverThreads[i].Start();
            }
        }

        private static void ServerThread()
        {
            do
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient(); // Blocks until a connection is made with a client

                string connectionID = Guid.NewGuid().ToString().Substring(0, 8); // Generate a new connection ID
                Console.WriteLine("[!] New Connection: " + connectionID);

                NetworkStream nStream = tcpClient.GetStream();

                Thread.Sleep(500); // Introducing a bit of serverside delay...

                // Read the first 4 bytes and convert to an integer. This is the length of the proceeding endpoint message
                byte[] endpointLengthBytes = new byte[4];
                nStream.Read(endpointLengthBytes, 0, 4);
                int endpointLength = BitConverter.ToInt32(endpointLengthBytes);

                // Read the number of bytes as above and decode to the endpoint string
                byte[] endpointBytes = new byte[endpointLength];
                nStream.Read(endpointBytes, 0, endpointLength);
                string endpoint = Encoding.ASCII.GetString(endpointBytes);

                // Read the next 4 bytes and convert to an integer. This is the length of the proceeding message
                byte[] messageLengthBytes = new byte[4];
                nStream.Read(messageLengthBytes, 0, 4);
                int messageLength = BitConverter.ToInt32(messageLengthBytes);

                // Read the number of bytes as above and decode to the message string
                byte[] messageBytes = new byte[messageLength];
                nStream.Read(messageBytes, 0, messageLength);
                string message = Encoding.ASCII.GetString(messageBytes);

                Console.WriteLine(connectionID + " Called Endpoint: " + endpoint);
                Console.WriteLine(connectionID + " Sent Message: " + message);

                string response = "";
                switch (endpoint) // Switch based on the endpoint called
                {
                    case "TaskOne":
                        response = TaskOne();
                        break;
                    case "TaskTwo":
                        response = TaskTwo();
                        break;
                    case "TaskThree":
                        response = TaskThree();
                        break;
                    default:
                        response = "Unknown Endpoint";
                        break;
                }

                byte[] rawData = SerializeResponse(response);
                nStream.Write(rawData, 0, rawData.Length);
            }
            while (true);
        }

        private static string TaskOne()
        {
            Thread.Sleep(10000); // Wait for 10 seconds
            return "Task 1 Finished!";
        }

        private static string TaskTwo()
        {
            Thread.Sleep(1000); // Wait for 1 second
            return "Task 2 Finished!";
        }

        private static string TaskThree()
        {
            Thread.Sleep(100);  // Wait for 100 milliseconds
            return "Task 3 Finished!";
        }

        private static byte[] SerializeResponse(string response)
        {
            // Convert string to byte array
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            // Get the number of bytes that the response uses as an integer and turn this into a 4 byte array
            int responseLength = responseBytes.Length;
            byte[] responseLengthBytes = BitConverter.GetBytes(responseLength);

            // Calculate the size of the byte array we will need to send this message.
            // +4 gives us space for one integer which will contain the length of the response message
            byte[] rawData = new byte[responseLength + 4];

            // Append the response length and response to the raw data byte array
            responseLengthBytes.CopyTo(rawData, 0);
            responseBytes.CopyTo(rawData, 4);

            return rawData;
        }
    }
}
