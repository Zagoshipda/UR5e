using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace URServer
{
    class Program
    {
        static void Main(string[] args)
        {

            // The IP address of the server (the PC on which this program is running)
            //string sHostIpAddress = "192.168.1.170";
            string sHostIpAddress = "127.0.0.1";
            int nPort = 21;     // Standard port number

            // The following names are used in the PolyScope script for referencing the working points:
            //const string csMsgPoint1 = "po1";       // Name of an arbitrary work point 1
            //const string csMsgPoint2 = "po2";       // Name of an arbitrary work point 2
            //const string csMsgPoint3 = "po3";       // Name of an arbitrary work point 3
            //const string csMsgPoint4 = "po4";       // Name of an arbitrary work point 4
            //const string csMsgPoint5 = "Point_5";   // Name of an arbitrary work point 5
            // ...

            string[] csMsgPoint = new string[] { "po1", "po2", "po3", "po4", "Point_5" };
            //public static readonly string[] csMsgPoint = { "po1", "po2", "po1", "po2", "Point_5" };
            //public enum csMsgPoint { po1, po2, po3, po4, Point_5 };

            Console.WriteLine("Opening IP Address: " + sHostIpAddress);
            // Create the IP address
            IPAddress ipAddress = IPAddress.Parse(sHostIpAddress);

            Console.WriteLine("Starting to listen on port: " + nPort);
            // Create the tcp Listener
            TcpListener tcpListener = new TcpListener(ipAddress, nPort);
            tcpListener.Start();    // Start listening

            // Keep on listening FOREVER (NO break).
            while (true)
            {
                // Accept the client. Wait until the client connects to the server.
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                
                Console.WriteLine("Accepted new client");

                // Open the network stream
                NetworkStream stream = tcpClient.GetStream();

                // Iterate while client is connected. If disconnected, break and wait for the next client.
                while (tcpClient.Client.Connected)
                {
                    // STREAM READ
                    // Create a byte array for the available bytes
                    byte[] arrayBytesRequest = new byte[tcpClient.Available];
                    Console.WriteLine($"tcpClient.Available : {tcpClient.Available}");  // 2 more than the length of request command because \r\n (new line character in Windows) follows after the string.

                    // Read the bytes from the stream.
                    // Also returns the total number of bytes read into the buffer.
                    int nRead = stream.Read(arrayBytesRequest, 0, arrayBytesRequest.Length);
                    // 1) if RECEIVED
                    if (nRead > 0)
                    {
                        // Convert the byte array into a string
                        string sMsgRequest = ASCIIEncoding.ASCII.GetString(arrayBytesRequest);
                        Console.WriteLine("Received message request: " + sMsgRequest);
                        
                        string sMsgAnswer = string.Empty;
                        // Check which workpoint is requested
                        if (sMsgRequest.Substring(0, nRead-2).Equals(csMsgPoint[0]))
                        {
                            // Some point in space for work point 1
                            sMsgAnswer = "(0.4, 0, 0.5, 0, -3.14159, 0)";
                        }
                        else if (sMsgRequest.Substring(0, nRead - 2).Equals(csMsgPoint[1]))
                        {
                            // Some point in space for work point 2
                            sMsgAnswer = "(0.3, 0.5, 0.5, 0, 3.14159, 0)"; ;
                        }
                        else if (sMsgRequest.Substring(0, nRead - 2).Equals(csMsgPoint[2]))
                        {
                            // Some point in space for work point 3
                            sMsgAnswer = "(0, 0.6, 0.5, 0, 3.14159, 0)";
                        }

                        // custom test
                        else if (sMsgRequest.Substring(0, nRead - 2).Equals(csMsgPoint[3]))
                        {
                            // Some point in space for work point 4
                            sMsgAnswer = "(-0.1, -0.1, -0.1, -0.1, -0.1, -0.1)";
                        }
                        else if (sMsgRequest.Substring(0, nRead - 2).Equals(csMsgPoint[4]))
                        {
                            // Some point in space for work point 5
                            sMsgAnswer = "(0.1, 0.1, 0.1, 0.1, 0.1, 0.1)";
                        }
                        else
                        {
                            //sMsgAnswer = "(0, 0, 0, 0, 0, 0)";  // default : go HOME ...?
                            sMsgAnswer = "";
                        }

                        if (sMsgAnswer.Length > 0)
                        {
                            Console.WriteLine("Sending message answer: " + sMsgAnswer);

                            // STREAM WRITE
                            // Convert the point into a byte array
                            byte[] arrayBytesAnswer = ASCIIEncoding.ASCII.GetBytes(sMsgAnswer);
                            // Send the byte array to the client
                            stream.Write(arrayBytesAnswer, 0, arrayBytesAnswer.Length);
                        }
                        else
                        {
                            Console.WriteLine("your command was WRONG.\n" +
                                "your command message should be : po1~4 or Point_5");
                        }
                    }
                    // 2. if NOT RECEIVED (nRead <= 0)
                    else
                    {
                        if (tcpClient.Available == 0)
                        {
                            Console.WriteLine("Client closed the connection.");
                            // NO bytes read, and NO bytes available, the client is closed.
                            stream.Close();     // also close the stream
                        }
                    }

                }
                Console.WriteLine("server waits for next client connection...");
            }
        }
    }
}
