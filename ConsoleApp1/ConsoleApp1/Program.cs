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
            Connection();
            Menu();
        }


        public static void Menu()
        {
            Console.WriteLine("");
            Console.WriteLine("                   Seleccione una opcion");
            Console.WriteLine("");
            Console.WriteLine("--1 ------------------- commit");
            Console.WriteLine("--2 ------------------- update");
            Console.WriteLine("--3 ------------------- Saber si es principal");
            string sOpcion = Console.ReadLine();

            switch (sOpcion)
            {
                case "1":

                    break;
                case "2":
                  
                    break;
                case "3":
                   
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

                IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAdress = iphostInfo.AddressList[0];
                IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, 8080);

                Socket client = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


                try
                {

                    client.Connect(ipEndpoint);
                    Console.WriteLine("Socket created to {0}", client.RemoteEndPoint.ToString());


                    while (true)
                    {

                        string message = Console.ReadLine();
                        byte[] sendmsg = Encoding.ASCII.GetBytes(message);
                        int n = client.Send(sendmsg);
                    }


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

