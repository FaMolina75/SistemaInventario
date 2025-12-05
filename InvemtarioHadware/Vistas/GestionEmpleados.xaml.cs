using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionEmpleados : Window
    {
        private int idEmpleadoSeleccionado = 0;

        public GestionEmpleados()
        {
            InitializeComponent();
            CargarEmpleados();
        }

        // ==========================================
        // ============ CARGAR DATOS ================
        // ==========================================

        private void CargarEmpleados()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM Empleados ORDER BY Nombre";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgEmpleados.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empleados: " + ex.Message);
            }
        }

        // ==========================================
        // ======== CREAR O ACTUALIZAR ==============
        // ==========================================

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del empleado es obligatorio.");
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

                    if (idEmpleadoSeleccionado == 0)
                    {
                        // CREAR NUEVO EMPLEADO
                        query = "INSERT INTO Empleados (Nombre, Puesto, Departamento) VALUES (@nom, @puesto, @depto)";
                        cmd.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@puesto", txtPuesto.Text.Trim());
                        cmd.Parameters.AddWithValue("@depto", txtDepto.Text.Trim());
                    }
                    else
                    {
                        // ACTUALIZAR EMPLEADO EXISTENTE
                        query = "UPDATE Empleados SET Nombre = @nom, Puesto = @puesto, Departamento = @depto WHERE idEmpleado = @id";
                        cmd.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@puesto", txtPuesto.Text.Trim());
                        cmd.Parameters.AddWithValue("@depto", txtDepto.Text.Trim());
                        cmd.Parameters.AddWithValue("@id", idEmpleadoSeleccionado);
                    }

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(idEmpleadoSeleccionado == 0 ? "Empleado agregado correctamente" : "Empleado actualizado correctamente");
                    LimpiarFormulario();
                    CargarEmpleados();
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
            idEmpleadoSeleccionado = Convert.ToInt32(row["idEmpleado"]);
            txtNombre.Text = row["Nombre"].ToString();
            txtPuesto.Text = row["Puesto"].ToString();
            txtDepto.Text = row["Departamento"].ToString();

            btnGuardar.Content = "ACTUALIZAR";
            btnCancelar.Visibility = Visibility.Visible;
        }

        // ==========================================
        // ============== ELIMINAR ==================
        // ==========================================

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            int id = Convert.ToInt32(row["idEmpleado"]);
            string nombre = row["Nombre"].ToString();

            if (MessageBox.Show($"¿Eliminar al empleado '{nombre}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        string query = "DELETE FROM Empleados WHERE idEmpleado = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        // Resetear AUTO_INCREMENT
                        ResetearAutoIncrement(con, "Empleados", "idEmpleado");

                        MessageBox.Show("Empleado eliminado");
                        LimpiarFormulario();
                        CargarEmpleados();
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
            txtNombre.Clear();
            txtPuesto.Clear();
            txtDepto.Clear();
            idEmpleadoSeleccionado = 0;
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