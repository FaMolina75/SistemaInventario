using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionProveedores : Window
    {
        public GestionProveedores()
        {
            InitializeComponent();
            CargarProveedores();
        }

        private void CargarProveedores()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM Proveedores";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgProveedores.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return;

            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Proveedores (NombreProveedor, Telefono) VALUES (@nom, @tel)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@tel", txtTelefono.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Proveedor Agregado");
                    txtNombre.Clear(); txtTelefono.Clear();
                    CargarProveedores();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}