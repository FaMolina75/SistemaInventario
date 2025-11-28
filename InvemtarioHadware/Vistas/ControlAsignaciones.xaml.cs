using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data; // Tu conexión

namespace InvemtarioHadware.Vistas
{
    public partial class ControlAsignaciones : Window
    {
        public ControlAsignaciones()
        {
            InitializeComponent();
            CargarDatos();
            dpFechaAsignacion.SelectedDate = DateTime.Now; // Pone la fecha de hoy por defecto
        }

        private void CargarDatos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // 1. Cargar Empleados para el ComboBox
                    string queryEmp = "SELECT idEmpleado, Nombre FROM Empleados";
                    MySqlDataAdapter daEmp = new MySqlDataAdapter(queryEmp, con);
                    DataTable dtEmp = new DataTable();
                    daEmp.Fill(dtEmp);
                    cbEmpleado.ItemsSource = dtEmp.DefaultView;

                    // 2. Cargar Hardware disponible (Concatenamos Serie y Modelo para que se vea bonito)
                    // CONCAT(h.Serie, ' - ', m.NombreModelo) crea un texto combinado
                    string queryHw = @"SELECT h.idHardware, CONCAT(h.Serie, ' - ', m.NombreModelo) AS SerieModelo 
                                       FROM Hardware h 
                                       JOIN Modelos m ON h.idModelo = m.idModelo";
                    // Nota: Podríamos agregar 'WHERE h.Estado = 'Operativo'' para ser más estrictos

                    MySqlDataAdapter daHw = new MySqlDataAdapter(queryHw, con);
                    DataTable dtHw = new DataTable();
                    daHw.Fill(dtHw);
                    cbHardware.ItemsSource = dtHw.DefaultView;

                    // 3. Cargar la Tabla de Historial (DataGrid)
                    string queryGrid = @"
                        SELECT a.idAsignacion, h.Serie, e.Nombre AS NombreEmpleado, a.FechaAsignacion, a.FechaDevolucion 
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

        // BOTÓN: REGISTRAR NUEVA ASIGNACIÓN
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
                    // Insertamos solo la fecha de asignación. La de devolución se queda NULL (significa que lo tienen actualmente)
                    string query = "INSERT INTO AsignacionesHW (FechaAsignacion, idHardware, idEmpleado) VALUES (@fecha, @hw, @emp)";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@fecha", dpFechaAsignacion.SelectedDate);
                    cmd.Parameters.AddWithValue("@hw", cbHardware.SelectedValue);
                    cmd.Parameters.AddWithValue("@emp", cbEmpleado.SelectedValue);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Equipo asignado correctamente.");

                    CargarDatos(); // Recargar tabla
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar: " + ex.Message);
            }
        }

        // BOTÓN: DEVOLVER EQUIPO (ACTUALIZAR FECHA DEVOLUCIÓN)
        private void btnDevolver_Click(object sender, RoutedEventArgs e)
        {
            // Verificamos si seleccionó algo en la tabla
            if (dgAsignaciones.SelectedItem is DataRowView fila)
            {
                // Si ya tiene fecha de devolución, no hacemos nada
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
                            // Actualizamos el registro poniendo la fecha de hoy como devolución
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
    }
}