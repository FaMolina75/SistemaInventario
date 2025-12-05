using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionUbicaciones : Window
    {
        // Variables para controlar si estamos editando (ID > 0) o creando (ID = 0)
        private int idSedeSeleccionada = 0;
        private int idUbicacionSeleccionada = 0;

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

        // ==========================================
        // ============== LOGICA SEDES ==============
        // ==========================================
        
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
                    cbSede.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Sedes: " + ex.Message); }
        }

        // CREAR O ACTUALIZAR SEDE
        private void btnGuardarSede_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSede.Text)) return;
            
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = con;

                    if (idSedeSeleccionada == 0)
                    {
                        // INSERTAR
                        query = "INSERT INTO Sedes (NombreSede) VALUES (@nom)";
                        cmd.Parameters.AddWithValue("@nom", txtSede.Text);
                    }
                    else
                    {
                        // ACTUALIZAR
                        query = "UPDATE Sedes SET NombreSede = @nom WHERE idSede = @id";
                        cmd.Parameters.AddWithValue("@nom", txtSede.Text);
                        cmd.Parameters.AddWithValue("@id", idSedeSeleccionada);
                    }

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(idSedeSeleccionada == 0 ? "Sede Agregada" : "Sede Actualizada");
                    LimpiarFormularioSede();
                    CargarSedes();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // PREPARAR PARA EDITAR SEDE
        private void btnEditarSede_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            idSedeSeleccionada = Convert.ToInt32(row["idSede"]);
            txtSede.Text = row["NombreSede"].ToString();
            btnGuardarSede.Content = "ACTUALIZAR";
            btnCancelarSede.Visibility = Visibility.Visible;
        }

        // ELIMINAR SEDE
        private void btnEliminarSede_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            int id = Convert.ToInt32(row["idSede"]);
            string nombre = row["NombreSede"].ToString();

            if (MessageBox.Show($"¿Estás seguro de eliminar la sede '{nombre}'?\nEsto podría borrar las ubicaciones asociadas.", 
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        string query = "DELETE FROM Sedes WHERE idSede = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        
                        // Resetear AUTO_INCREMENT
                        ResetearAutoIncrement(con, "Sedes", "idSede");
                        
                        MessageBox.Show("Sede eliminada");
                        LimpiarFormularioSede();
                        CargarSedes();
                        CargarUbicaciones();
                    }
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("No se puede eliminar. Verifique que no tenga datos asociados.\nError: " + ex.Message); 
                }
            }
        }

        private void btnCancelarSede_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormularioSede();
        }

        private void LimpiarFormularioSede()
        {
            txtSede.Clear();
            idSedeSeleccionada = 0;
            btnGuardarSede.Content = "AGREGAR SEDE";
            btnCancelarSede.Visibility = Visibility.Collapsed;
        }

        // ==========================================
        // =========== LOGICA UBICACIONES ===========
        // ==========================================
        
        private void CargarUbicaciones()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = @"SELECT u.idUbicacion, u.NombreUbicacion, u.idSede, s.NombreSede 
                                     FROM Ubicaciones u
                                     JOIN Sedes s ON u.idSede = s.idSede";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgUbicaciones.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Ubicaciones: " + ex.Message); }
        }

        // CREAR O ACTUALIZAR UBICACION
        private void btnGuardarUbicacion_Click(object sender, RoutedEventArgs e)
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
                    string query = "";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = con;

                    if (idUbicacionSeleccionada == 0)
                    {
                        // INSERT
                        query = "INSERT INTO Ubicaciones (NombreUbicacion, idSede) VALUES (@nom, @sede)";
                        cmd.Parameters.AddWithValue("@nom", txtUbicacion.Text);
                        cmd.Parameters.AddWithValue("@sede", cbSede.SelectedValue);
                    }
                    else
                    {
                        // UPDATE
                        query = "UPDATE Ubicaciones SET NombreUbicacion = @nom, idSede = @sede WHERE idUbicacion = @id";
                        cmd.Parameters.AddWithValue("@nom", txtUbicacion.Text);
                        cmd.Parameters.AddWithValue("@sede", cbSede.SelectedValue);
                        cmd.Parameters.AddWithValue("@id", idUbicacionSeleccionada);
                    }

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(idUbicacionSeleccionada == 0 ? "Ubicación Agregada" : "Ubicación Actualizada");
                    LimpiarFormularioUbicacion();
                    CargarUbicaciones();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // PREPARAR PARA EDITAR UBICACION
        private void btnEditarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            idUbicacionSeleccionada = Convert.ToInt32(row["idUbicacion"]);
            txtUbicacion.Text = row["NombreUbicacion"].ToString();
            cbSede.SelectedValue = row["idSede"];
            btnGuardarUbicacion.Content = "ACTUALIZAR";
            btnCancelarUbicacion.Visibility = Visibility.Visible;
        }

        // ELIMINAR UBICACION
        private void btnEliminarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            int id = Convert.ToInt32(row["idUbicacion"]);
            string nombre = row["NombreUbicacion"].ToString();

            if (MessageBox.Show($"¿Eliminar la ubicación '{nombre}'?", "Confirmar", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        string query = "DELETE FROM Ubicaciones WHERE idUbicacion = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        
                        // Resetear AUTO_INCREMENT
                        ResetearAutoIncrement(con, "Ubicaciones", "idUbicacion");
                        
                        MessageBox.Show("Ubicación eliminada");
                        LimpiarFormularioUbicacion();
                        CargarUbicaciones();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
            }
        }

        private void btnCancelarUbicacion_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormularioUbicacion();
        }

        private void LimpiarFormularioUbicacion()
        {
            txtUbicacion.Clear();
            cbSede.SelectedIndex = -1;
            idUbicacionSeleccionada = 0;
            btnGuardarUbicacion.Content = "GUARDAR UBICACIÓN";
            btnCancelarUbicacion.Visibility = Visibility.Collapsed;
        }

        // ==========================================
        // ========== MÉTODO AUXILIAR ===============
        // ==========================================
        
        private void ResetearAutoIncrement(MySqlConnection con, string tabla, string columnaId)
        {
            try
            {
                // Obtener el máximo ID actual
                string queryMax = $"SELECT MAX({columnaId}) FROM {tabla}";
                MySqlCommand cmdMax = new MySqlCommand(queryMax, con);
                object resultado = cmdMax.ExecuteScalar();
                
                int nuevoAutoIncrement = 1;
                if (resultado != DBNull.Value && resultado != null)
                {
                    nuevoAutoIncrement = Convert.ToInt32(resultado) + 1;
                }
                
                // Resetear AUTO_INCREMENT al siguiente valor disponible
                string queryReset = $"ALTER TABLE {tabla} AUTO_INCREMENT = {nuevoAutoIncrement}";
                MySqlCommand cmdReset = new MySqlCommand(queryReset, con);
                cmdReset.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al resetear AUTO_INCREMENT: {ex.Message}");
            }
        }
    }
}