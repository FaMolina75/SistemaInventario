using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data; // Ahora sí reconocerá esto


namespace InvemtarioHadware.Vistas
{
    public partial class GestionHardware : Window
    {
        private int idHardwareSeleccionado = 0;

        public GestionHardware()
        {
            InitializeComponent();
            CargarCombos();
            CargarGrid();
        }

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
                    string query = @"SELECT h.idHardware, h.Serie, m.NombreModelo, u.NombreUbicacion, h.Estado, h.FechaCompra, h.idModelo, h.idUbicacion 
                                     FROM Hardware h
                                     JOIN Modelos m ON h.idModelo = m.idModelo
                                     JOIN Ubicaciones u ON h.idUbicacion = u.idUbicacion";
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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSerie.Text) || cbModelo.SelectedValue == null || cbUbicacion.SelectedValue == null)
            {
                MessageBox.Show("Completa todos los campos.");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = (idHardwareSeleccionado == 0)
                        ? "INSERT INTO Hardware (Serie, idModelo, idUbicacion, FechaCompra, Estado) VALUES (@serie, @modelo, @ubicacion, @fecha, @estado)"
                        : "UPDATE Hardware SET Serie=@serie, idModelo=@modelo, idUbicacion=@ubicacion, FechaCompra=@fecha, Estado=@estado WHERE idHardware=@id";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@serie", txtSerie.Text);
                    cmd.Parameters.AddWithValue("@modelo", cbModelo.SelectedValue);
                    cmd.Parameters.AddWithValue("@ubicacion", cbUbicacion.SelectedValue);
                    cmd.Parameters.AddWithValue("@fecha", dpFechaCompra.SelectedDate ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@estado", cbEstado.Text);
                    if (idHardwareSeleccionado > 0) cmd.Parameters.AddWithValue("@id", idHardwareSeleccionado);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Guardado.");
                    LimpiarFormulario();
                    CargarGrid();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (idHardwareSeleccionado == 0) return;
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        MySqlCommand cmd = new MySqlCommand("DELETE FROM Hardware WHERE idHardware=@id", con);
                        cmd.Parameters.AddWithValue("@id", idHardwareSeleccionado);
                        cmd.ExecuteNonQuery();
                        LimpiarFormulario();
                        CargarGrid();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void dgHardware_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgHardware.SelectedItem is DataRowView fila)
            {
                idHardwareSeleccionado = Convert.ToInt32(fila["idHardware"]);
                txtSerie.Text = fila["Serie"].ToString();
                cbModelo.SelectedValue = fila["idModelo"];
                cbUbicacion.SelectedValue = fila["idUbicacion"];
                cbEstado.Text = fila["Estado"].ToString();
                if (fila["FechaCompra"] != DBNull.Value) dpFechaCompra.SelectedDate = Convert.ToDateTime(fila["FechaCompra"]);
            }
        }

        private void LimpiarFormulario()
        {
            txtSerie.Clear();
            cbModelo.SelectedIndex = -1;
            cbUbicacion.SelectedIndex = -1;
            dpFechaCompra.SelectedDate = null;
            cbEstado.SelectedIndex = 0;
            idHardwareSeleccionado = 0;
        }
    }
}