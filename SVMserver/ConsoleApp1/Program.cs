using ConsoleApp1.Model;
using Newtonsoft.Json;
using SVM_SA.Util;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SVM_SA
{
    class Program
    {
        private static int staPort = 80;

        static void Main(string[] args)
        {
            string port;
            Console.WriteLine("Ingrese puerto");
            port = Console.ReadLine();
            staPort = Convert.ToInt32(port);
            Menu(port);

        }

        public static void Menu(string port)
        {
            Console.WriteLine("");
            Console.WriteLine("                   Seleccione una opcion");
            Console.WriteLine("");
            Console.WriteLine("--1 ------------------- Inicializar Aplicativo");
            Console.WriteLine("--2 ------------------- Hacer este servidor principal");
            Console.WriteLine("--3 ------------------- Iniciar el servidor");
            Console.WriteLine("--3 ------------------- Saber si es principal");
            string sOpcion = Console.ReadLine();

            switch (sOpcion)
            {
                case "1":
                    Initialize(port);
                    break;
                case "2":
                    MakePrincipal(port);
                    break;
                case "3":
                    Conectar(port);
                    break;
                case "4":
                    if (IsPrincipal(port) == 1)
                    {
                        Console.WriteLine("Este servidor es el principal");
                    }
                    else
                    {
                        Console.WriteLine("Este servidor NO es el principal");
                    }
                    break;
                default:
                    Console.Clear();
                    Menu(port);
                    break;
            }
            Console.ReadLine();
        }

        private static void Conectar(string port, bool exit = false)
        {
            //Obtengo puerto



            //Creo tabla para almacenar la config


            Socket miPrimerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // paso 2 - creamos el socket
            IPEndPoint miDireccion = new IPEndPoint(IPAddress.Any, Convert.ToInt32(port));
            //paso 3 -IPAddress.Any significa que va a escuchar al cliente en toda la red 
            try
            {
                // paso 4
                miPrimerSocket.Bind(miDireccion); // Asociamos el socket a miDireccion
                while (true)
                {
                    miPrimerSocket.Listen(1); // Lo ponemos a escucha

                    Console.WriteLine("Escuchando por puerto " + port + " ...");
                    Socket Escuchar = miPrimerSocket.Accept();
                    Console.WriteLine("Conectado con exito");


                    //Verifico la opcion enviada
                    var oData = OReceive_from_Client(Escuchar);

                    switch (oData.id)
                    {
                        case 1:
                            Commit(oData);
                            break;

                        default:
                            Console.Clear();
                            break;
                    }

                }

            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            }
            Console.WriteLine("Presione cualquier tecla para terminar");
            Console.ReadLine();

        }

        private static void MakePrincipal(string port)
        {

            int result = -1;
            string ip = GenericFunction.GetLocalIPAddress();
            var conn = QuerySqlite.sql_con();
            conn.Open();

            SQLiteCommand cmd = new SQLiteCommand(QuerySqlite.UpdateConfIsPrincipal, conn);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@Inprincipal", 1);
            cmd.Parameters.AddWithValue("@InIp", ip);
            cmd.Parameters.AddWithValue("@InPort", port);
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                result = -2;
                Console.WriteLine(e.Message + "  " + e.ErrorCode.ToString());
            }
            catch (Exception e)
            {
                result = -2;
                Console.WriteLine(e.Message);
            }
            conn.Close();


        }

        private static int CreateTable_ConfSVM()
        {
            int result = -1;
            var conn = QuerySqlite.sql_con(true);
            try
            {

                conn.Open();

                SQLiteCommand createSqlite = new SQLiteCommand(QuerySqlite.CreateTableConf, conn);
                try
                {
                    result = createSqlite.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    result = -2;
                    Console.WriteLine(e.Message + "  " + e.ErrorCode.ToString());
                }
                catch (Exception e)
                {
                    result = -2;
                    Console.WriteLine(e.Message);
                }

            }
            catch (Exception ex)
            {

                result = -2;
                Console.WriteLine(ex.Message);
            }
            conn.Close();
            return result;
        }

        private static int Insert_ConfSVM(int port)
        {
            int result = -1;
            string ip = GenericFunction.GetLocalIPAddress();
            result = -3;
            var conn = QuerySqlite.sql_con();
            conn.Open();

            SQLiteCommand cmd = new SQLiteCommand(QuerySqlite.InsertConf, conn);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@Inprincipal", 0);
            cmd.Parameters.AddWithValue("@InIp", ip);
            cmd.Parameters.AddWithValue("@InPort", port);
            cmd.Parameters.AddWithValue("@InConex", DateTime.Now.ToString());
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                result = -2;
                Console.WriteLine(e.Message + "  " + e.ErrorCode.ToString());
            }
            catch (Exception e)
            {
                result = -2;
                Console.WriteLine(e.Message);
            }
            conn.Close();
            return result;
        }

        private static void Initialize(string port)
        {
            int iErrror = 0;
            iErrror = CreateTable_ConfSVM();
            Console.WriteLine("CreateTable_ConfSVM" + iErrror.ToString());
            Insert_ConfSVM(Convert.ToInt32(port));
            Console.WriteLine("Insert_ConfSVM " + iErrror.ToString());
        }

        private static int IsPrincipal(string port)
        {
            string ip = GenericFunction.GetLocalIPAddress();
            var conn = QuerySqlite.sql_con();
            conn.Open();

            int isPrincipal = 0;

            SQLiteCommand cmd = new SQLiteCommand(QuerySqlite.SelectIsPrincipal, conn);
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@InIp", ip);
            cmd.Parameters.AddWithValue("@InPort", port);
            try
            {
                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        isPrincipal = Convert.ToInt16(rdr["isPrincipal"]);
                    }
                }
            }
            catch (SQLiteException e)
            {

                Console.WriteLine(e.Message + "  " + e.ErrorCode.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            conn.Close();
            return isPrincipal;

        }

        private static string Receive_from_Client(Socket socket)
        {
            byte[] b = new byte[80];
            string sResp = "";
            int k = socket.Receive(b);
            Console.WriteLine("Recieved...");
            for (int i = 0; i < k; i++)
                sResp = sResp + Convert.ToChar(b[i]);
            return sResp;
        }

        private static void Send_to_Client(string message, Socket s)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes(message));
            Console.WriteLine("respuesta enviada");
        }


        private static void Commit(GeneralDTO odata)
        {
            ConvertFile(odata.data,odata.nameFile);
        }

        private static void ConvertFile(string file, string name)
        {
            byte[] sfile = Convert.FromBase64String(file);
            string hh = GenericFunction.GetExecutingDirectoryName() + name;
            File.WriteAllBytes(hh, sfile);

            if (IsPrincipal(staPort.ToString()) == 1)
            {
                var ips = ipList();

                ips.ForEach(x =>
                {
                    IPAddress address = IPAddress.Parse(x);
                    sendToSa(staPort, hh, address);
                });

            }



        }


        private static void Update(Socket s)
        {
            Send_to_Client("Update realizado!", s);
            Console.WriteLine("Update realizado");
        }

        private static void GetPrincipal(Socket s, string port)
        {
            string result = IsPrincipal(port).ToString();
            Send_to_Client(result, s);
            Console.WriteLine("Es el principal =" + result);
        }

        private static void Receive_File(Socket socket)
        {
            byte[] b = new byte[100];
            string sResp = "";
            int k = socket.Receive(b);
            Console.WriteLine("Recieved...");
            for (int i = 0; i < k; i++)
                sResp = sResp + Convert.ToChar(b[i]);
            File.WriteAllBytes(@"C:\Users\Joselyn\Documents\Repositorios\2018\SVM\ConsoleApp1\hola.txt", b);
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


        private static void sendToSa(int port, string spath, IPAddress iPAddress)
        {
            byte[] data = new byte[10];
            IPEndPoint ipEndpoint = new IPEndPoint(iPAddress, port);
            Socket client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(ipEndpoint);
                byte[] filebyte = File.ReadAllBytes(spath);
                string temp_inBase64 = Convert.ToBase64String(filebyte);
                var obj = new GeneralDTO { data = temp_inBase64, id = 1 };
                var sendmsg = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                int n = client.Send(sendmsg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static List<string> ipList()
        {
            List<string> ipList = new List<string>();
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            StreamReader file = new StreamReader(GenericFunction.GetExecutingDirectoryName() + @"iplist.txt");
            while ((line = file.ReadLine()) != null)
            {
                ipList.Add(line);

                
                Console.WriteLine("There were {0} lines.", counter);
                // Suspend the screen.  
                Console.ReadLine();
            }
            file.Close();
            return ipList;
        }
    }
}
