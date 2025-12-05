using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionUsuarios : Window
    {
        private int idUsuarioSeleccionado = 0;

        public GestionUsuarios()
        {
            InitializeComponent();
            CargarRoles();
            CargarUsuarios();
        }

        // ==========================================
        // ============ CARGAS INICIALES ============
        // ==========================================

        private void CargarRoles()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "SELECT * FROM Roles";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cbRol.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Roles: " + ex.Message); }
        }

        private void CargarUsuarios()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Username, u.idRol, r.NombreRol 
                                     FROM Usuarios u
                                     JOIN Roles r ON u.idRol = r.idRol";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgUsuarios.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar Usuarios: " + ex.Message); }
        }

        // ==========================================
        // ========== CREAR O ACTUALIZAR ============
        // ==========================================

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || cbRol.SelectedValue == null)
            {
                MessageBox.Show("Completa el nombre de usuario y selecciona un rol.");
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

                    if (idUsuarioSeleccionado == 0)
                    {
                        // CREAR NUEVO USUARIO
                        if (string.IsNullOrWhiteSpace(txtPassword.Password))
                        {
                            MessageBox.Show("Ingresa una contraseña para el nuevo usuario.");
                            return;
                        }

                        string passEncriptada = GetSHA256(txtPassword.Password);
                        query = "INSERT INTO Usuarios (Username, PasswordHash, idRol) VALUES (@user, @pass, @rol)";
                        cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@pass", passEncriptada);
                        cmd.Parameters.AddWithValue("@rol", cbRol.SelectedValue);
                    }
                    else
                    {
                        // ACTUALIZAR USUARIO EXISTENTE
                        if (string.IsNullOrWhiteSpace(txtPassword.Password))
                        {
                            // No cambiar contraseña
                            query = "UPDATE Usuarios SET Username = @user, idRol = @rol WHERE idUsuario = @id";
                            cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                            cmd.Parameters.AddWithValue("@rol", cbRol.SelectedValue);
                            cmd.Parameters.AddWithValue("@id", idUsuarioSeleccionado);
                        }
                        else
                        {
                            // Cambiar contraseña también
                            string passEncriptada = GetSHA256(txtPassword.Password);
                            query = "UPDATE Usuarios SET Username = @user, PasswordHash = @pass, idRol = @rol WHERE idUsuario = @id";
                            cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                            cmd.Parameters.AddWithValue("@pass", passEncriptada);
                            cmd.Parameters.AddWithValue("@rol", cbRol.SelectedValue);
                            cmd.Parameters.AddWithValue("@id", idUsuarioSeleccionado);
                        }
                    }

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(idUsuarioSeleccionado == 0 ? "Usuario creado exitosamente" : "Usuario actualizado exitosamente");
                    LimpiarFormulario();
                    CargarUsuarios();
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
            idUsuarioSeleccionado = Convert.ToInt32(row["idUsuario"]);
            txtUsername.Text = row["Username"].ToString();
            cbRol.SelectedValue = row["idRol"];
            txtPassword.Clear();
            
            btnGuardar.Content = "ACTUALIZAR";
            btnCancelar.Visibility = Visibility.Visible;
            lblInfoPass.Visibility = Visibility.Visible;
        }

        // ==========================================
        // ============== ELIMINAR ==================
        // ==========================================

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)((Button)sender).DataContext;
            int id = Convert.ToInt32(row["idUsuario"]);
            string username = row["Username"].ToString();

            if (MessageBox.Show($"¿Eliminar el usuario '{username}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        string query = "DELETE FROM Usuarios WHERE idUsuario = @id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        // Resetear AUTO_INCREMENT
                        ResetearAutoIncrement(con, "Usuarios", "idUsuario");

                        MessageBox.Show("Usuario eliminado");
                        LimpiarFormulario();
                        CargarUsuarios();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
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
            txtUsername.Clear();
            txtPassword.Clear();
            cbRol.SelectedIndex = -1;
            idUsuarioSeleccionado = 0;
            btnGuardar.Content = "CREAR";
            btnCancelar.Visibility = Visibility.Collapsed;
            lblInfoPass.Visibility = Visibility.Collapsed;
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

        public static string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}