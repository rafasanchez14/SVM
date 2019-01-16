using SVM_SA.Util;
using System;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SVM_SA
{
    class Program
    {
        static void Main(string[] args)
        {
            string port;
            Console.WriteLine("Ingrese puerto");
            port = Console.ReadLine();
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

        private static void Conectar(string port)
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
                miPrimerSocket.Listen(1); // Lo ponemos a escucha

                Console.WriteLine("Escuchando por puerto " + port + " ...");
                Socket Escuchar = miPrimerSocket.Accept();
                //creamos el nuevo socket, para comenzar a trabajar con él
                //La aplicación queda en reposo hasta que el socket se conecte a el cliente
                //Una vez conectado, la aplicación sigue su camino  
                Console.WriteLine("Conectado con exito");

               string sOpcion = Receive_from_Client(Escuchar);

















                /*Aca ponemos todo lo que queramos hacer con el socket, osea antes de 
                cerrarlo je*/
                miPrimerSocket.Close(); //Luego lo cerramos

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
            byte[] b = new byte[100];
            string sResp = "";
            int k = socket.Receive(b);
            Console.WriteLine("Recieved...");
            for (int i = 0; i < k; i++)
                sResp= sResp + Convert.ToChar(b[i]);
            return sResp;
        }

        private static void Send_to_Client(string message, Socket s)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes(message));
            Console.WriteLine("respuesta enviada");
        }

    }
}
