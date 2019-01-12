using SVM_SA.Util;
using System;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;

namespace SVM_SA
{
    class Program
    {
        static void Main(string[] args)
        {
            Conectar();
        }

        public static void Conectar()
        {
            //Obtengo puerto
            string port;
            int iErrror = 0;
            Console.WriteLine("Ingrese puerto");
            port = Console.ReadLine();

            //Creo tabla para almacenar la config
            iErrror = CreateTable_ConfSVM();
            Console.WriteLine("CreateTable_ConfSVM" + iErrror.ToString());


            Insert_ConfSVM(Convert.ToInt32(port), AddressFamily.InterNetwork.ToString(),DateTime.Now.ToString());
            Console.WriteLine("Insert_ConfSVM " + iErrror.ToString());

            Socket miPrimerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // paso 2 - creamos el socket
            IPEndPoint miDireccion = new IPEndPoint(IPAddress.Any, Convert.ToInt32(port));
            //paso 3 -IPAddress.Any significa que va a escuchar al cliente en toda la red 
            try
            {
                // paso 4
                miPrimerSocket.Bind(miDireccion); // Asociamos el socket a miDireccion
                miPrimerSocket.Listen(1); // Lo ponemos a escucha

                Console.WriteLine("Escuchando por puerto "+ port+" ...");
                Socket Escuchar = miPrimerSocket.Accept();
                //creamos el nuevo socket, para comenzar a trabajar con él
                //La aplicación queda en reposo hasta que el socket se conecte a el cliente
                //Una vez conectado, la aplicación sigue su camino  
                Console.WriteLine("Conectado con exito");

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

        public static void MakePrincipal(int port, string ip, string conex)
        {

                int result = -1;
                var conn = QuerySqlite.sql_con();
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(QuerySqlite.UpdateConfIsPrincipal,conn);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Inprincipal", 1);
                cmd.Parameters.AddWithValue("@InIp", ip);
                cmd.Parameters.AddWithValue("@InPort", port);
                try
                {
                    result = cmd.ExecuteNonQuery();
                }
                catch (SQLiteException)
                {

                }
                conn.Close();


        }

        private static int CreateTable_ConfSVM()
        {
            int result = -1;
            var conn = QuerySqlite.sql_con();
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
               
            }
            catch (Exception ex)
            {

                result = -2;
                Console.WriteLine(ex.Message);
            }
            conn.Close();
            return result;
        }

        public static int Insert_ConfSVM(int port, string ip, string conex)
        {
            int result = -1;

                result = -3;
                var conn = QuerySqlite.sql_con();
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand(QuerySqlite.InsertConf, conn);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Inprincipal", 0);
                cmd.Parameters.AddWithValue("@InIp", ip);
                cmd.Parameters.AddWithValue("@InPort", port);
                cmd.Parameters.AddWithValue("@InConex", conex);
                try
                {
                    result = cmd.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    result = -2;
                    Console.WriteLine(e.Message + "  " + e.ErrorCode.ToString());
                }
                conn.Close();
            return result;
        }

    }
}
