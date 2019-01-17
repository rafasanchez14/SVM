using ConsoleApp1.Model;
using Newtonsoft.Json;
using SVM_SA.Util;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1
{

    public class Cliente
    {
        public Cliente()
        {
        }

        static void Main(string[] args)
        {

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



        public static void Commit()
        {
            Console.WriteLine("Opcion elegida commit");

            Console.WriteLine("En caso de no habrlo hecho Coloque su archivo en la carpeta Files");

            Console.WriteLine("Introduzca el nombre de su archivo(incluyendo extension)");

            string sNameFile = Console.ReadLine();


            byte[] data = new byte[10];

            IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = iphostInfo.AddressList[0];
            IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, 9595);
            Socket client = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(ipEndpoint);

            string  sNameFiledate = DateTime.Now.ToString("yyyyMMddHHmmss") +sNameFile;
            string path = GenericFunction.GetExecutingDirectoryName() + sNameFile;
            Console.WriteLine("Ruta seleccionada: " + path);


            byte[] filebyte = File.ReadAllBytes(path);

            string temp_inBase64 = Convert.ToBase64String(filebyte);
            var obj = new GeneralDTO { data = temp_inBase64, id = 1, nameFile = sNameFiledate};
            var sendmsg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

            Console.WriteLine("Presione una tecla para Confirmar commit");
            Console.ReadKey();

            int n = client.Send(sendmsg);
            Console.WriteLine("Transmission end.");
            Console.ReadKey();
        }

    }
}

