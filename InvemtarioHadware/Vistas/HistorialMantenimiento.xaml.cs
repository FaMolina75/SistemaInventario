using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class HistorialMantenimiento : Window
    {
        public HistorialMantenimiento()
        {
            InitializeComponent();
            CargarDatos();
            dpFecha.SelectedDate = DateTime.Now;
        }

        private void CargarDatos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // 1. Cargar Hardware
                    string queryHw = @"SELECT h.idHardware, CONCAT(h.Serie, ' - ', m.NombreModelo) AS SerieModelo 
                                       FROM Hardware h 
                                       JOIN Modelos m ON h.idModelo = m.idModelo";
                    MySqlDataAdapter daHw = new MySqlDataAdapter(queryHw, con);
                    DataTable dtHw = new DataTable();
                    daHw.Fill(dtHw);
                    cbHardware.ItemsSource = dtHw.DefaultView;

                    // 2. Cargar Proveedores
                    string queryProv = "SELECT idProveedor, NombreProveedor FROM Proveedores";
                    MySqlDataAdapter daProv = new MySqlDataAdapter(queryProv, con);
                    DataTable dtProv = new DataTable();
                    daProv.Fill(dtProv);
                    cbProveedor.ItemsSource = dtProv.DefaultView;

                    // 3. Cargar Tabla Mantenimientos
                    string queryGrid = @"
                        SELECT m.Fecha, h.Serie, p.NombreProveedor, m.Descripcion, m.Costo 
                        FROM MantenimientosHW m
                        JOIN Hardware h ON m.idHardware = h.idHardware
                        JOIN Proveedores p ON m.idProveedor = p.idProveedor
                        ORDER BY m.Fecha DESC";
                    MySqlDataAdapter daGrid = new MySqlDataAdapter(queryGrid, con);
                    DataTable dtGrid = new DataTable();
                    daGrid.Fill(dtGrid);
                    dgMantenimientos.ItemsSource = dtGrid.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos: " + ex.Message);
            }
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (cbHardware.SelectedValue == null || cbProveedor.SelectedValue == null || string.IsNullOrWhiteSpace(txtCosto.Text))
            {
                MessageBox.Show("Completa todos los campos obligatorios.");
                return;
            }

            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // PASO 1: Insertar el registro de mantenimiento
                    string queryInsert = @"INSERT INTO MantenimientosHW (Fecha, Descripcion, Costo, idHardware, idProveedor) 
                                           VALUES (@fecha, @desc, @costo, @hw, @prov)";

                    MySqlCommand cmd = new MySqlCommand(queryInsert, con);
                    cmd.Parameters.AddWithValue("@fecha", dpFecha.SelectedDate);
                    cmd.Parameters.AddWithValue("@desc", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@costo", Convert.ToDecimal(txtCosto.Text));
                    cmd.Parameters.AddWithValue("@hw", cbHardware.SelectedValue);
                    cmd.Parameters.AddWithValue("@prov", cbProveedor.SelectedValue);
                    cmd.ExecuteNonQuery();

                    // PASO 2: Actualizar el estado del equipo a 'En Reparación'
                    string queryUpdate = "UPDATE Hardware SET Estado = 'En Reparación' WHERE idHardware = @hw";
                    MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, con);
                    cmdUpdate.Parameters.AddWithValue("@hw", cbHardware.SelectedValue);
                    cmdUpdate.ExecuteNonQuery();

                    MessageBox.Show("Mantenimiento registrado. El equipo ahora está 'En Reparación'.");

                    // Limpieza
                    txtCosto.Clear();
                    txtDescripcion.Clear();
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message);
            }
        }
        // Agrega este método dentro de la clase
        private void btnNuevoProveedor_Click(object sender, RoutedEventArgs e)
        {
            // 1. Abrir la ventanita de proveedores como un diálogo (bloquea la de atrás hasta que cierres)
            GestionProveedores ventana = new GestionProveedores();
            ventana.ShowDialog();

            // 2. Al cerrar esa ventana, recargamos la lista para que aparezca el nuevo que acabas de meter
            CargarDatos();
        }
    }
}