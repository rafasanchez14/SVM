using ConsoleApp1.Model;
using Newtonsoft.Json;
using SVM_SA.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace ConsoleApp1
{

    public class Cliente
    {
        private static int staPort = 80;
        private static IPAddress iPAddress;
        private static int trys = 0;
        public Cliente()
        {
        }

        static void Main(string[] args)
        {

            MenuFunc();
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
                    Update();
                    break;


                default:
                    Console.Clear();
                    Menu();
                    break;
            }

            Console.Clear();
            Menu();
            Console.ReadLine();
        }

        public static void MenuFunc()
        {
            Console.WriteLine("Elegir puerto");
            string Port = Console.ReadLine();
            staPort = Convert.ToInt32(Port);
            Menu();
        }

        public static void Commit()
        {

            try
            {
                Get_Ip();
                Console.WriteLine("Opcion elegida commit");

                Console.WriteLine("En caso de no haberlo hecho Coloque su archivo en la carpeta Files");

                Console.WriteLine("Introduzca el nombre de su archivo(incluyendo extension)");

                string sNameFile = Console.ReadLine();


                byte[] data = new byte[10];
                IPEndPoint ipEndpoint = new IPEndPoint(iPAddress, staPort);
                Socket client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                client.Connect(ipEndpoint);

                string sNameFiledate = DateTime.Now.ToString("yyyyMMddHHmmss") + sNameFile;
                string path = GenericFunction.GetExecutingDirectoryName() + sNameFile;
                Console.WriteLine("Ruta seleccionada: " + path);


                byte[] filebyte = File.ReadAllBytes(path);

                string temp_inBase64 = Convert.ToBase64String(filebyte);
                var obj = new GeneralDTO { data = temp_inBase64, id = 1, nameFile = sNameFiledate };
                var sendmsg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

                Console.WriteLine("Presione una tecla para Confirmar commit");
                Console.ReadKey();

                int n = client.Send(sendmsg);
                Console.WriteLine("Commit enviado");
                SVMLogger.Write(GenericFunction.GetLocalIPAddress() +"  Ha realizado un commit", "SVMActivity");
            }
            catch (SocketException ex)
            {
                SVMLogger.Write("Ocurrio una exception controlada Socket: " + ex.Message);
                Console.WriteLine("Servidor caido eligiendo otro...");
                Commit();
               
            }
            catch (Exception ex)
            {
                Commit();
                SVMLogger.Write("Ocurrio una exception: " + ex.Message + MethodBase.GetCurrentMethod());
                
            }

        }


        public static void Update()
        {
            try
            {
                Get_Ip();
                Console.WriteLine("Introduzca el nombre de su archivo(incluyendo extension)");

                string sNameFile = Console.ReadLine();

                byte[] data = new byte[10];

                IPEndPoint ipEndpoint = new IPEndPoint(iPAddress, staPort);
                Socket client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                client.Connect(ipEndpoint);

                var obj = new GeneralDTO { data = "aaaaa", id = 2, nameFile = sNameFile };
                var sendmsg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                int n = client.Send(sendmsg);
                client.Close();
                //Empiezo a escuchar

                Socket miPrimerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint miDireccion = new IPEndPoint(IPAddress.Any, Convert.ToInt32(staPort) + 1);
                miPrimerSocket.Bind(miDireccion);
                miPrimerSocket.Listen(1);

                Console.WriteLine("Escuchando por puerto " + staPort + " ...");
                Socket Escuchar = miPrimerSocket.Accept();
                var oData = OReceive_from_Client(Escuchar);
                Console.WriteLine("Archivo recibido "+oData.data);

                byte[] sfile = Convert.FromBase64String(oData.data);
                string hh = GenericFunction.GetExecutingDirectoryName() + oData.nameFile;
                File.WriteAllBytes(hh, sfile);
                SVMLogger.Write(GenericFunction.GetLocalIPAddress() + "  Ha realizado un update", "SVMActivity");
                Console.ReadKey();
            }
            catch (SocketException ex)
            {
                SVMLogger.Write("Ocurrio una exception controlada Socket: " + ex.Message);
            }
            catch (Exception ex)
            {
                SVMLogger.Write("Ocurrio una exception: " + ex.Message + MethodBase.GetCurrentMethod());

            }



        }

        private static GeneralDTO OReceive_from_Client(Socket socket)
        {
            byte[] b = new byte[1000];
            string sResp = "";
            int k = socket.Receive(b);
            for (int i = 0; i < k; i++)
                sResp = sResp + Convert.ToChar(b[i]);
            var results = JsonConvert.DeserializeObject<GeneralDTO>(sResp);
            return results;
        }

        private static List<string> ipList()
        {
            List<string> ipList = new List<string>();
            StreamReader file = new StreamReader(GenericFunction.GetExecutingDirectoryNameIp() + @"iplist.txt");
            string line;
            try
            {
                // Read the file and display it line by line.  
                 while ((line = file.ReadLine()) != null)
                 {
                    ipList.Add(line);

                 }
            }
            catch (Exception ex)
            {
                SVMLogger.Write("Ocurrio una exception: " + ex.Message + MethodBase.GetCurrentMethod());
            }
            
            file.Close();
            return ipList;
        }

        private static void Get_Ip()
        {
            try
            {
                var iplist = ipList();
                if (trys > iplist.Count - 1)
                {
                    trys = 0;
                }
                else
                {
                    iPAddress = IPAddress.Parse(iplist[trys]);
                    trys = trys + 1;
                }
            }
            catch (Exception ex)
            {
                SVMLogger.Write("Ocurrio una exception: " + ex.Message + MethodBase.GetCurrentMethod());
            }


        }


        
    }
}

