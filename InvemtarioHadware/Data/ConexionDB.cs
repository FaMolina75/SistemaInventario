using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;

namespace InvemtarioHadware.Data
{
    public class ConexionDB
    {
        // Configuración para XAMPP (root sin contraseña)
        private string connectionString = "Server=localhost;Database=InventarioHardwareDB;Uid=root;Pwd=;";
        private MySqlConnection connection;

        public ConexionDB()
        {
            connection = new MySqlConnection(connectionString);
        }

        // AQUI ESTABA EL ERROR: Es 'MySqlConnection', no 'MySqlSqlConnection'
        public MySqlConnection GetConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection;
        }

        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public bool ProbarConexion()
        {
            try
            {
                GetConnection();
                CloseConnection();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}