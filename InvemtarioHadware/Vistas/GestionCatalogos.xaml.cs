using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionCatalogos : Window
    {
        public GestionCatalogos()
        {
            InitializeComponent();
            CargarTodo();
        }

        private void CargarTodo()
        {
            CargarMarcas();
            CargarTipos();
            CargarModelos();
        }

        // --- 1. GESTIÓN DE MARCAS ---
        private void CargarMarcas()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM Marcas";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgMarcas.ItemsSource = dt.DefaultView;
                    cbMarcaModelo.ItemsSource = dt.DefaultView; // Llenamos también el combo de la pestaña 3
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Marcas: " + ex.Message); }
        }

        private void btnMarca_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMarca.Text)) return;
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Marcas (NombreMarca) VALUES (@nom)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtMarca.Text);
                    cmd.ExecuteNonQuery();
                    txtMarca.Clear();
                    CargarMarcas(); // Recargar para verla en la lista y en el combo
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // --- 2. GESTIÓN DE TIPOS ---
        private void CargarTipos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM TiposHardware";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgTipos.ItemsSource = dt.DefaultView;
                    cbTipoModelo.ItemsSource = dt.DefaultView; // Llenamos el combo de la pestaña 3
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Tipos: " + ex.Message); }
        }

        private void btnTipo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTipo.Text)) return;
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO TiposHardware (NombreTipo) VALUES (@nom)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtTipo.Text);
                    cmd.ExecuteNonQuery();
                    txtTipo.Clear();
                    CargarTipos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // --- 3. GESTIÓN DE MODELOS ---
        private void CargarModelos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // JOIN para mostrar nombres en lugar de números
                    string query = @"SELECT m.NombreModelo, ma.NombreMarca, t.NombreTipo 
                                     FROM Modelos m
                                     JOIN Marcas ma ON m.idMarca = ma.idMarca
                                     JOIN TiposHardware t ON m.idTipoHardware = t.idTipoHardware";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgModelos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Modelos: " + ex.Message); }
        }

        private void btnModelo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModelo.Text) || cbMarcaModelo.SelectedValue == null || cbTipoModelo.SelectedValue == null)
            {
                MessageBox.Show("Faltan datos");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Modelos (NombreModelo, idMarca, idTipoHardware) VALUES (@nom, @marca, @tipo)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtModelo.Text);
                    cmd.Parameters.AddWithValue("@marca", cbMarcaModelo.SelectedValue);
                    cmd.Parameters.AddWithValue("@tipo", cbTipoModelo.SelectedValue);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Modelo creado correctamente");
                    txtModelo.Clear();
                    CargarModelos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}