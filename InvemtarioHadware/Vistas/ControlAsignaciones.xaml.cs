using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class ControlAsignaciones : Window
    {
        private int idAsignacionSeleccionada = 0;

        public ControlAsignaciones()
        {
            InitializeComponent();
            CargarDatos();
            // CONFIGURAR FECHA MÍNIMA (HOY)
            dpFechaAsignacion.DisplayDateStart = DateTime.Today; // No permite seleccionar ayer
            dpFechaAsignacion.SelectedDate = DateTime.Today;     // Por defecto selecciona hoy
        }

        // ==========================================
        // ============ CARGAR DATOS ================
        // ==========================================

        private void CargarDatos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // 1. Cargar Empleados
                    string queryEmp = "SELECT idEmpleado, Nombre FROM Empleados";
                    MySqlDataAdapter daEmp = new MySqlDataAdapter(queryEmp, con);
                    DataTable dtEmp = new DataTable();
                    daEmp.Fill(dtEmp);
                    cbEmpleado.ItemsSource = dtEmp.DefaultView;

                    // 2. Cargar Hardware
                    string queryHw = @"SELECT h.idHardware, CONCAT(h.Serie, ' - ', m.NombreModelo) AS SerieModelo 
                                       FROM Hardware h 
                                       JOIN Modelos m ON h.idModelo = m.idModelo";
                    MySqlDataAdapter daHw = new MySqlDataAdapter(queryHw, con);
                    DataTable dtHw = new DataTable();
                    daHw.Fill(dtHw);
                    cbHardware.ItemsSource = dtHw.DefaultView;

                    // 3. Cargar Historial
                    string queryGrid = @"SELECT a.idAsignacion, a.idHardware, a.idEmpleado, h.Serie, 
                                         e.Nombre AS NombreEmpleado, a.FechaAsignacion, a.FechaDevolucion 
                                         FROM AsignacionesHW a
                                         JOIN Hardware h ON a.idHardware = h.idHardware
                                         JOIN Empleados e ON a.idEmpleado = e.idEmpleado
                                         ORDER BY a.FechaAsignacion DESC";
                    MySqlDataAdapter daGrid = new MySqlDataAdapter(queryGrid, con);
                    DataTable dtGrid = new DataTable();
                    daGrid.Fill(dtGrid);
                    dgAsignaciones.ItemsSource = dtGrid.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos: " + ex.Message);
            }
        }

        // ==========================================
        // ======== CREAR O ACTUALIZAR ==============
        // ==========================================

        private void btnAsignar_Click(object sender, RoutedEventArgs e)
        {
            if (cbHardware.SelectedValue == null || cbEmpleado.SelectedValue == null || dpFechaAsignacion.SelectedDate == null)
            {
                MessageBox.Show("Selecciona Equipo, Empleado y Fecha.");
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

                    if (idAsignacionSeleccionada == 0)
                    {
                        // CREAR NUEVA ASIGNACIÓN
                        query = "INSERT INTO AsignacionesHW (FechaAsignacion, idHardware, idEmpleado) VALUES (@fecha, @hw, @emp)";
                        cmd.Parameters.AddWithValue("@fecha", dpFechaAsignacion.SelectedDate);
                        cmd.Parameters.AddWithValue("@hw", cbHardware.SelectedValue);
                        cmd.Parameters.AddWithValue("@emp", cbEmpleado.SelectedValue);
                    }
                    else
                    {
                        // ACTUALIZAR ASIGNACIÓN EXISTENTE
                        query = "UPDATE AsignacionesHW SET FechaAsignacion = @fecha, idHardware = @hw, idEmpleado = @emp WHERE idAsignacion = @id";
                        cmd.Parameters.AddWithValue("@fecha", dpFechaAsignacion.SelectedDate);
                        cmd.Parameters.AddWithValue("@hw", cbHardware.SelectedValue);
                        cmd.Parameters.AddWithValue("@emp", cbEmpleado.SelectedValue);
                        cmd.Parameters.AddWithValue("@id", idAsignacionSeleccionada);
                    }

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(idAsignacionSeleccionada == 0 ? "Asignación registrada correctamente" : "Asignación actualizada correctamente");
                    LimpiarFormulario();
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ==========================================
        // ============== EDITAR ====================
        // ==========================================

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            idAsignacionSeleccionada = Convert.ToInt32(row["idAsignacion"]);
            cbHardware.SelectedValue = row["idHardware"];
            cbEmpleado.SelectedValue = row["idEmpleado"];
            dpFechaAsignacion.SelectedDate = Convert.ToDateTime(row["FechaAsignacion"]);

            btnAsignar.Content = "ACTUALIZAR";
            btnCancelar.Visibility = Visibility.Visible;
        }

        // ==========================================
        // ============== ELIMINAR ==================
        // ==========================================

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            int id = Convert.ToInt32(row["idAsignacion"]);
            string serie = row["Serie"].ToString();
            string empleado = row["NombreEmpleado"].ToString();

            if (MessageBox.Show($"¿Eliminar la asignación del equipo '{serie}' a '{empleado}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        string query = "DELETE FROM AsignacionesHW WHERE idAsignacion = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        // Resetear AUTO_INCREMENT
                        ResetearAutoIncrement(con, "AsignacionesHW", "idAsignacion");

                        MessageBox.Show("Asignación eliminada");
                        LimpiarFormulario();
                        CargarDatos();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
            }
        }

        // ==========================================
        // ============== DEVOLVER ==================
        // ==========================================

        private void btnDevolver_Click(object sender, RoutedEventArgs e)
        {
            if (dgAsignaciones.SelectedItem is DataRowView fila)
            {
                if (fila["FechaDevolucion"] != DBNull.Value)
                {
                    MessageBox.Show("Este equipo ya fue devuelto anteriormente.");
                    return;
                }

                int idAsignacion = Convert.ToInt32(fila["idAsignacion"]);

                if (MessageBox.Show("¿Confirmar devolución del equipo?", "Devolución", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        ConexionDB db = new ConexionDB();
                        using (MySqlConnection con = db.GetConnection())
                        {
                            string query = "UPDATE AsignacionesHW SET FechaDevolucion = @fecha WHERE idAsignacion = @id";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                            cmd.Parameters.AddWithValue("@id", idAsignacion);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Equipo marcado como devuelto.");
                            CargarDatos();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona una asignación ACTIVA de la tabla para devolver.");
            }
        }

        // ==========================================
        // ========== GESTIONAR EMPLEADOS ===========
        // ==========================================

        private void btnGestionarEmpleados_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Intentar abrir la ventana de gestión de empleados si existe
                GestionEmpleados ventana = new GestionEmpleados();
                ventana.ShowDialog();
                // Al cerrar, recargamos la lista para ver los cambios
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir la ventana de gestión de empleados.\n" + ex.Message);
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
            cbHardware.SelectedIndex = -1;
            cbEmpleado.SelectedIndex = -1;
            dpFechaAsignacion.SelectedDate = DateTime.Today;
            idAsignacionSeleccionada = 0;
            btnAsignar.Content = "REGISTRAR";
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