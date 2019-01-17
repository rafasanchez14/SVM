using ConsoleApp1.Model;
using Newtonsoft.Json;
using SVM_SA.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

            Menu();
        }


        public static void Menu()
        {
            
            Console.WriteLine("Elegir puerto");
            string Port = Console.ReadLine();
            staPort = Convert.ToInt32(Port);
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

            Console.ReadLine();
        }



        public static void Commit()
        {

            try
            {
                Get_Ip();
                Console.WriteLine("Opcion elegida commit");

                Console.WriteLine("En caso de no habrlo hecho Coloque su archivo en la carpeta Files");

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
                Console.WriteLine("Transmission end.");
                Console.ReadKey();
            }
            catch (SocketException ex)
            {

                Commit();
                Console.WriteLine("Intento otro.");
            }
            catch (Exception ex)
            {
                Commit();
                Console.WriteLine("exec otro.");
                throw;
            }

        }


        public static void Update()
        {
            Console.WriteLine("Introduzca el nombre de su archivo(incluyendo extension)");

            string sNameFile = Console.ReadLine();

            byte[] data = new byte[10];

            IPEndPoint ipEndpoint = new IPEndPoint(iPAddress, staPort);
            Socket client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(ipEndpoint);

            var obj = new GeneralDTO { data = "aaaaa", id = 2, nameFile = sNameFile};
            var sendmsg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
            int n = client.Send(sendmsg);
            client.Close();
            //Empiezo a escuchar

            Socket miPrimerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint miDireccion = new IPEndPoint(IPAddress.Any, Convert.ToInt32(staPort)+1);
            miPrimerSocket.Bind(miDireccion);
            miPrimerSocket.Listen(1);
            Console.WriteLine("Escuchando por puerto " + staPort + " ...");
            Socket Escuchar = miPrimerSocket.Accept();
            var oData = OReceive_from_Client(Escuchar);
            Console.WriteLine(oData.data);

            byte[] sfile = Convert.FromBase64String(oData.data);
            string hh = GenericFunction.GetExecutingDirectoryName() + oData.nameFile;
            File.WriteAllBytes(hh, sfile);
        }

        private static GeneralDTO OReceive_from_Client(Socket socket)
        {
            byte[] b = new byte[1000];
            string sResp = "";
            int k = socket.Receive(b);
            Console.WriteLine("Recieved...");
            for (int i = 0; i < k; i++)
                sResp = sResp + Convert.ToChar(b[i]);

            var results = JsonConvert.DeserializeObject<GeneralDTO>(sResp);

            return results;
        }

        private static List<string> ipList()
        {
            List<string> ipList = new List<string>();
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            StreamReader file = new StreamReader(GenericFunction.GetExecutingDirectoryNameIp() + @"iplist.txt");
            while ((line = file.ReadLine()) != null)
            {
                ipList.Add(line);

            }
            file.Close();
            return ipList;
        }

        private static void Get_Ip()
        {
            var iplist = ipList();
            if (trys > iplist.Count-1)
            {
                trys = 0;
            }
            else
            {
                iPAddress = IPAddress.Parse(iplist[trys]);
                trys = trys + 1;
            }
            
        }

    }
}

