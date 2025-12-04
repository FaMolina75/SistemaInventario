using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionCatalogos : Window
    {
        private DataTable dtMarcasOriginal;
        private DataTable dtTiposOriginal;
        private DataTable dtModelosOriginal;

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

        // ==================== GESTIÓN DE MARCAS ====================
        
        private void CargarMarcas()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM Marcas ORDER BY NombreMarca";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    dtMarcasOriginal = new DataTable();
                    da.Fill(dtMarcasOriginal);
                    dgMarcas.ItemsSource = dtMarcasOriginal.DefaultView;
                    cbMarcaModelo.ItemsSource = dtMarcasOriginal.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Marcas: " + ex.Message); }
        }

        private void dgMarcas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMarcas.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgMarcas.SelectedItem;
                txtIdMarca.Text = row["idMarca"].ToString();
                txtMarca.Text = row["NombreMarca"].ToString();
            }
        }

        private void btnAgregarMarca_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MessageBox.Show("Ingrese el nombre de la marca");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Marcas (NombreMarca) VALUES (@nom)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtMarca.Text.Trim());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Marca agregada correctamente");
                    LimpiarMarca();
                    CargarMarcas();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al agregar: " + ex.Message); }
        }

        private void btnActualizarMarca_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdMarca.Text))
            {
                MessageBox.Show("Seleccione una marca del listado");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MessageBox.Show("Ingrese el nombre de la marca");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "UPDATE Marcas SET NombreMarca = @nom WHERE idMarca = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtMarca.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", txtIdMarca.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Marca actualizada correctamente");
                    LimpiarMarca();
                    CargarMarcas();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private void btnEliminarMarca_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdMarca.Text))
            {
                MessageBox.Show("Seleccione una marca del listado");
                return;
            }
            
            var result = MessageBox.Show("¿Está seguro de eliminar esta marca?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;

            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "DELETE FROM Marcas WHERE idMarca = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", txtIdMarca.Text);
                    cmd.ExecuteNonQuery();
                    
                    // Resetear AUTO_INCREMENT
                    ResetearAutoIncrement(con, "Marcas", "idMarca");
                    
                    MessageBox.Show("Marca eliminada correctamente");
                    LimpiarMarca();
                    CargarMarcas();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
        }

        private void btnLimpiarMarca_Click(object sender, RoutedEventArgs e)
        {
            LimpiarMarca();
        }

        private void LimpiarMarca()
        {
            txtIdMarca.Clear();
            txtMarca.Clear();
            txtBuscarMarca.Clear();
            dgMarcas.SelectedItem = null;
            CargarMarcas(); // Reestablecer datos de la tabla
        }

        private void txtBuscarMarca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtMarcasOriginal == null) return;
            
            string filtro = txtBuscarMarca.Text.Trim();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgMarcas.ItemsSource = dtMarcasOriginal.DefaultView;
            }
            else
            {
                DataView dv = dtMarcasOriginal.DefaultView;
                dv.RowFilter = $"NombreMarca LIKE '%{filtro}%'";
                dgMarcas.ItemsSource = dv;
            }
        }

        // ==================== GESTIÓN DE TIPOS ====================
        
        private void CargarTipos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM TiposHardware ORDER BY NombreTipo";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    dtTiposOriginal = new DataTable();
                    da.Fill(dtTiposOriginal);
                    dgTipos.ItemsSource = dtTiposOriginal.DefaultView;
                    cbTipoModelo.ItemsSource = dtTiposOriginal.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Tipos: " + ex.Message); }
        }

        private void dgTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTipos.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgTipos.SelectedItem;
                txtIdTipo.Text = row["idTipoHardware"].ToString();
                txtTipo.Text = row["NombreTipo"].ToString();
            }
        }

        private void btnAgregarTipo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTipo.Text))
            {
                MessageBox.Show("Ingrese el nombre del tipo");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO TiposHardware (NombreTipo) VALUES (@nom)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtTipo.Text.Trim());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Tipo agregado correctamente");
                    LimpiarTipo();
                    CargarTipos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al agregar: " + ex.Message); }
        }

        private void btnActualizarTipo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdTipo.Text))
            {
                MessageBox.Show("Seleccione un tipo del listado");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTipo.Text))
            {
                MessageBox.Show("Ingrese el nombre del tipo");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "UPDATE TiposHardware SET NombreTipo = @nom WHERE idTipoHardware = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtTipo.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", txtIdTipo.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Tipo actualizado correctamente");
                    LimpiarTipo();
                    CargarTipos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private void btnEliminarTipo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdTipo.Text))
            {
                MessageBox.Show("Seleccione un tipo del listado");
                return;
            }
            
            var result = MessageBox.Show("¿Está seguro de eliminar este tipo?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;

            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "DELETE FROM TiposHardware WHERE idTipoHardware = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", txtIdTipo.Text);
                    cmd.ExecuteNonQuery();
                    
                    // Resetear AUTO_INCREMENT
                    ResetearAutoIncrement(con, "TiposHardware", "idTipoHardware");
                    
                    MessageBox.Show("Tipo eliminado correctamente");
                    LimpiarTipo();
                    CargarTipos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
        }

        private void btnLimpiarTipo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarTipo();
        }

        private void LimpiarTipo()
        {
            txtIdTipo.Clear();
            txtTipo.Clear();
            txtBuscarTipo.Clear();
            dgTipos.SelectedItem = null;
            CargarTipos(); // Reestablecer datos de la tabla
        }

        private void txtBuscarTipo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtTiposOriginal == null) return;
            
            string filtro = txtBuscarTipo.Text.Trim();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgTipos.ItemsSource = dtTiposOriginal.DefaultView;
            }
            else
            {
                DataView dv = dtTiposOriginal.DefaultView;
                dv.RowFilter = $"NombreTipo LIKE '%{filtro}%'";
                dgTipos.ItemsSource = dv;
            }
        }

        // ==================== GESTIÓN DE MODELOS ====================
        
        private void CargarModelos()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = @"SELECT m.idModelo, m.NombreModelo, ma.NombreMarca, t.NombreTipo, m.idMarca, m.idTipoHardware
                                     FROM Modelos m
                                     JOIN Marcas ma ON m.idMarca = ma.idMarca
                                     JOIN TiposHardware t ON m.idTipoHardware = t.idTipoHardware
                                     ORDER BY m.NombreModelo";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    dtModelosOriginal = new DataTable();
                    da.Fill(dtModelosOriginal);
                    dgModelos.ItemsSource = dtModelosOriginal.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Modelos: " + ex.Message); }
        }

        private void dgModelos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgModelos.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgModelos.SelectedItem;
                txtIdModelo.Text = row["idModelo"].ToString();
                txtModelo.Text = row["NombreModelo"].ToString();
                cbMarcaModelo.SelectedValue = row["idMarca"];
                cbTipoModelo.SelectedValue = row["idTipoHardware"];
            }
        }

        private void btnAgregarModelo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModelo.Text) || cbMarcaModelo.SelectedValue == null || cbTipoModelo.SelectedValue == null)
            {
                MessageBox.Show("Complete todos los campos");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Modelos (NombreModelo, idMarca, idTipoHardware) VALUES (@nom, @marca, @tipo)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtModelo.Text.Trim());
                    cmd.Parameters.AddWithValue("@marca", cbMarcaModelo.SelectedValue);
                    cmd.Parameters.AddWithValue("@tipo", cbTipoModelo.SelectedValue);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Modelo agregado correctamente");
                    LimpiarModelo();
                    CargarModelos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al agregar: " + ex.Message); }
        }

        private void btnActualizarModelo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdModelo.Text))
            {
                MessageBox.Show("Seleccione un modelo del listado");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtModelo.Text) || cbMarcaModelo.SelectedValue == null || cbTipoModelo.SelectedValue == null)
            {
                MessageBox.Show("Complete todos los campos");
                return;
            }
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "UPDATE Modelos SET NombreModelo = @nom, idMarca = @marca, idTipoHardware = @tipo WHERE idModelo = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nom", txtModelo.Text.Trim());
                    cmd.Parameters.AddWithValue("@marca", cbMarcaModelo.SelectedValue);
                    cmd.Parameters.AddWithValue("@tipo", cbTipoModelo.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", txtIdModelo.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Modelo actualizado correctamente");
                    LimpiarModelo();
                    CargarModelos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private void btnEliminarModelo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdModelo.Text))
            {
                MessageBox.Show("Seleccione un modelo del listado");
                return;
            }
            
            var result = MessageBox.Show("¿Está seguro de eliminar este modelo?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;

            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "DELETE FROM Modelos WHERE idModelo = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", txtIdModelo.Text);
                    cmd.ExecuteNonQuery();
                    
                    // Resetear AUTO_INCREMENT
                    ResetearAutoIncrement(con, "Modelos", "idModelo");
                    
                    MessageBox.Show("Modelo eliminado correctamente");
                    LimpiarModelo();
                    CargarModelos();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
        }

        private void btnLimpiarModelo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarModelo();
        }

        private void LimpiarModelo()
        {
            txtIdModelo.Clear();
            txtModelo.Clear();
            txtBuscarModelo.Clear();
            cbMarcaModelo.SelectedIndex = -1;
            cbTipoModelo.SelectedIndex = -1;
            dgModelos.SelectedItem = null;
            CargarModelos(); // Reestablecer datos de la tabla
        }

        private void txtBuscarModelo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtModelosOriginal == null) return;
            
            string filtro = txtBuscarModelo.Text.Trim();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgModelos.ItemsSource = dtModelosOriginal.DefaultView;
            }
            else
            {
                DataView dv = dtModelosOriginal.DefaultView;
                dv.RowFilter = $"NombreModelo LIKE '%{filtro}%' OR NombreMarca LIKE '%{filtro}%' OR NombreTipo LIKE '%{filtro}%'";
                dgModelos.ItemsSource = dv;
            }
        }

        // ==================== MÉTODO AUXILIAR ====================
        
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
                // No mostramos error al usuario, solo registramos internamente
                System.Diagnostics.Debug.WriteLine($"Error al resetear AUTO_INCREMENT: {ex.Message}");
            }
        }
    }
}