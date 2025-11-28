using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionUbicaciones : Window
    {
        public GestionUbicaciones()
        {
            InitializeComponent();
            CargarTodo();
        }

        private void CargarTodo()
        {
            CargarSedes();
            CargarUbicaciones();
        }

        // --- SEDES ---
        private void CargarSedes()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM Sedes";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgSedes.ItemsSource = dt.DefaultView;
                    cbSede.ItemsSource = dt.DefaultView; // Llenamos el combo de la otra pestaña
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Sedes: " + ex.Message); }
        }

        private void btnSede_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSede.Text)) return;
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Sedes (NombreSede) VALUES (@nom)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtSede.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Sede Agregada");
                    txtSede.Clear();
                    CargarSedes(); // Actualiza grids y combos
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // --- UBICACIONES ---
        private void CargarUbicaciones()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // JOIN para ver el nombre de la Sede en lugar del ID
                    string query = @"SELECT u.NombreUbicacion, s.NombreSede 
                                     FROM Ubicaciones u
                                     JOIN Sedes s ON u.idSede = s.idSede";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgUbicaciones.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Ubicaciones: " + ex.Message); }
        }

        private void btnUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUbicacion.Text) || cbSede.SelectedValue == null)
            {
                MessageBox.Show("Escribe el nombre y selecciona la Sede.");
                return;
            }

            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Ubicaciones (NombreUbicacion, idSede) VALUES (@nom, @sede)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtUbicacion.Text);
                    cmd.Parameters.AddWithValue("@sede", cbSede.SelectedValue);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Ubicación Agregada");
                    txtUbicacion.Clear();
                    CargarUbicaciones();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}