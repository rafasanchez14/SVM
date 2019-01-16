using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System;
using System.Net;
using System.Net.Sockets;


namespace ConsoleApp1
{
  
        public class Cliente
        {
            public Cliente()
            {
            }

        static void Main(string[] args)
        {
            // Connection();
         
            Menu();
        }


        public static void Menu()
        {
            Console.WriteLine("");
            Console.WriteLine("                   Seleccione una opcion");
            Console.WriteLine("");
            Console.WriteLine("--1 ------------------- commit");
            Console.WriteLine("--2 ------------------- update");

            string sOpcion = Console.ReadLine();

            switch (sOpcion)
            {
                case "1":
                    Commit();

                    break;
                case "2":
                  
                    break;
                              
            
                default:
                    Console.Clear();
                    Menu();
                    break;
            }
            Console.ReadLine();
        }


        public static void Connection()
            {
                byte[] data = new byte[10];
            /*
                IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAdress = iphostInfo.AddressList[0];
                IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, 9595);*/
             IPHostEntry iphostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAdress = iphostInfo.AddressList[0];
            IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, 11000);

                Socket client = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


                try
                {

                    client.Connect(ipEndpoint);
                  //  Console.WriteLine("Socket created to {0}", client.RemoteEndPoint.ToString());
                  Console.WriteLine("Conectado");

                    /*while (true)
                    {

                        string message = Console.ReadLine();
                        byte[] sendmsg = Encoding.ASCII.GetBytes(message);
                        int n = client.Send(sendmsg);
                    }*/


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Console.WriteLine("Transmission end.");
                Console.ReadKey();

            }
        public static void Commit()
        {

            try
            {
                // Establish the local endpoint for the socket.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

                // Create a TCP socket.
                Socket client = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint.
                client.Connect(ipEndPoint);

                // There is a text file test.txt located in the root directory.
                string fileName = "C:\\+test.txt";

                // Send file fileName to remote device
                Console.WriteLine("Sending {0} to the host.", fileName);
                client.SendFile(fileName);

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Transmission end.");
            Console.ReadKey();

        }

 


    }

    }

