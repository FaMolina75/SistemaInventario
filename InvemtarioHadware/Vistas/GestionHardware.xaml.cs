using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionHardware : Window
    {
        private int idHardwareSeleccionado = 0;

        public GestionHardware()
        {
            InitializeComponent();
            // RESTRICCIÓN DE FECHAS (CORREGIDO):
            // Lógica estándar de compra:
            // - Permitir el pasado (para registrar compras antiguas).
            // - Bloquear el futuro (DisplayDateEnd = Hoy), ya que no puedes comprar mañana.
            dpFechaCompra.DisplayDateEnd = DateTime.Now;
            CargarCombos();
            CargarGrid();
        }

        // ==========================================
        // ============ CARGAR DATOS ================
        // ==========================================

        private void CargarCombos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // Cargar Modelos
                    string queryModelos = "SELECT idModelo, NombreModelo FROM Modelos";
                    MySqlDataAdapter daModelos = new MySqlDataAdapter(queryModelos, con);
                    DataTable dtModelos = new DataTable();
                    daModelos.Fill(dtModelos);
                    cbModelo.ItemsSource = dtModelos.DefaultView;

                    // Cargar Ubicaciones
                    string queryUbicaciones = "SELECT idUbicacion, NombreUbicacion FROM Ubicaciones";
                    MySqlDataAdapter daUbicaciones = new MySqlDataAdapter(queryUbicaciones, con);
                    DataTable dtUbicaciones = new DataTable();
                    daUbicaciones.Fill(dtUbicaciones);
                    cbUbicacion.ItemsSource = dtUbicaciones.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error cargando combos: " + ex.Message); }
        }

        private void CargarGrid()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = @"SELECT h.idHardware, h.Serie, m.NombreModelo, u.NombreUbicacion, 
                                     h.Estado, h.FechaCompra, h.idModelo, h.idUbicacion 
                                     FROM Hardware h
                                     JOIN Modelos m ON h.idModelo = m.idModelo
                                     JOIN Ubicaciones u ON h.idUbicacion = u.idUbicacion
                                     ORDER BY h.Serie";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgHardware.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tabla: " + ex.Message);
            }
        }

        // ==========================================
        // ======== CREAR O ACTUALIZAR ==============
        // ==========================================

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSerie.Text) || cbModelo.SelectedValue == null || cbUbicacion.SelectedValue == null)
            {
                MessageBox.Show("Completa los campos obligatorios (Serie, Modelo, Ubicación).");
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

                    if (idHardwareSeleccionado == 0)
                    {
                        // CREAR NUEVO HARDWARE - Incluye fecha
                        query = "INSERT INTO Hardware (Serie, idModelo, idUbicacion, FechaCompra, Estado) VALUES (@serie, @modelo, @ubicacion, @fecha, @estado)";
                        cmd.Parameters.AddWithValue("@serie", txtSerie.Text.Trim());
                        cmd.Parameters.AddWithValue("@modelo", cbModelo.SelectedValue);
                        cmd.Parameters.AddWithValue("@ubicacion", cbUbicacion.SelectedValue);
                        cmd.Parameters.AddWithValue("@fecha", dpFechaCompra.SelectedDate ?? DateTime.Now);
                        cmd.Parameters.AddWithValue("@estado", cbEstado.Text);
                    }
                    else
                    {
                        // ACTUALIZAR HARDWARE - NO modifica la fecha (seguridad)
                        query = "UPDATE Hardware SET Serie=@serie, idModelo=@modelo, idUbicacion=@ubicacion, Estado=@estado WHERE idHardware=@id";
                        cmd.Parameters.AddWithValue("@serie", txtSerie.Text.Trim());
                        cmd.Parameters.AddWithValue("@modelo", cbModelo.SelectedValue);
                        cmd.Parameters.AddWithValue("@ubicacion", cbUbicacion.SelectedValue);
                        cmd.Parameters.AddWithValue("@estado", cbEstado.Text);
                        cmd.Parameters.AddWithValue("@id", idHardwareSeleccionado);
                    }

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(idHardwareSeleccionado == 0 ? "Hardware registrado correctamente" : "Hardware actualizado correctamente");
                    LimpiarFormulario();
                    CargarGrid();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // ==========================================
        // ============== EDITAR ====================
        // ==========================================

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            idHardwareSeleccionado = Convert.ToInt32(row["idHardware"]);
            txtSerie.Text = row["Serie"].ToString();
            cbModelo.SelectedValue = row["idModelo"];
            cbUbicacion.SelectedValue = row["idUbicacion"];
            cbEstado.Text = row["Estado"].ToString();
            
            if (row["FechaCompra"] != DBNull.Value)
                dpFechaCompra.SelectedDate = Convert.ToDateTime(row["FechaCompra"]);
            else
                dpFechaCompra.SelectedDate = null;

            // Bloquear edición de fecha (seguridad)
            dpFechaCompra.IsEnabled = false;

            btnGuardar.Content = "ACTUALIZAR";
            btnCancelar.Visibility = Visibility.Visible;
        }

        // ==========================================
        // ============== ELIMINAR ==================
        // ==========================================

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            int id = Convert.ToInt32(row["idHardware"]);
            string serie = row["Serie"].ToString();

            if (MessageBox.Show($"¿Eliminar el hardware con serie '{serie}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        string query = "DELETE FROM Hardware WHERE idHardware = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        // Resetear AUTO_INCREMENT
                        ResetearAutoIncrement(con, "Hardware", "idHardware");

                        MessageBox.Show("Hardware eliminado");
                        LimpiarFormulario();
                        CargarGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        // ==========================================
        // ============== CANCELAR ==================
        // ==========================================

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            txtSerie.Clear();
            cbModelo.SelectedIndex = -1;
            cbUbicacion.SelectedIndex = -1;
            dpFechaCompra.SelectedDate = null;
            dpFechaCompra.IsEnabled = true; // Reactivar el control de fecha
            cbEstado.SelectedIndex = 0;
            idHardwareSeleccionado = 0;
            btnGuardar.Content = "GUARDAR";
            btnCancelar.Visibility = Visibility.Collapsed;
        }

        // ==========================================
        // ========== MÉTODOS AUXILIARES ============
        // ==========================================

        private void ResetearAutoIncrement(MySqlConnection con, string tabla, string columnaId)
        {
            try
            {
                string queryMax = $"SELECT MAX({columnaId}) FROM {tabla}";
                MySqlCommand cmdMax = new MySqlCommand(queryMax, con);
                object resultado = cmdMax.ExecuteScalar();

                int nuevoAutoIncrement = 1;
                if (resultado != DBNull.Value && resultado != null)
                {
                    nuevoAutoIncrement = Convert.ToInt32(resultado) + 1;
                }

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