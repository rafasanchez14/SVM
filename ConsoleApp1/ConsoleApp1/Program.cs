using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ConsoleApp1.Model;
using Newtonsoft.Json;

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
         
           // Menu();
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
            
                IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAdress = iphostInfo.AddressList[0];
                IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, 9595);
          

                Socket client = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


                try
                {

                    client.Connect(ipEndpoint);
                  //  Console.WriteLine("Socket created to {0}", client.RemoteEndPoint.ToString());
                  Console.WriteLine("Conectado");


                //ruta del file
                        string path = @"C:\Users\Usuario\Desktop\svm\test.txt";

                //convierto el file a bytearray
                        byte[] filebyte = File.ReadAllBytes(path);
                //luego a base64 para mandarlo al server
                        string temp_inBase64 = Convert.ToBase64String(filebyte);

                //lleno mi objeto General dto con la data y el id(opcion del menu)
                //tambien se puede obj.id="hola"

                        var obj = new GeneralDTO { data = temp_inBase64, id = 1 };

                //Convierto a json ese objeto y luego todo eso a bytearray
                         var sendmsg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
               
                //Mando byte array
                int n =  client.Send(sendmsg);

                //alla hago lo inverso para obtener todo
                //mandamelo asi por opciones en el id para yo saber que funcion quieres hacer
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
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 9595);

                // Create a TCP socket.
                Socket client = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint.
                client.Connect(ipEndPoint);

                // There is a text file test.txt located in the root directory.
                //  string fileName = "C:\\+test.txt";
                string fileName = @"C:\Users\Usuario\Desktop\svm\test.txt";
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

