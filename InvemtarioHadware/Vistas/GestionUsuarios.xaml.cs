using System;
using System.Data;
using System.Security.Cryptography; // Necesario para la encriptación
using System.Text;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class GestionUsuarios : Window
    {
        public GestionUsuarios()
        {
            InitializeComponent();
            CargarRoles();
            CargarUsuarios();
        }

        // --- CARGAS INICIALES ---
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
            catch (Exception ex) { MessageBox.Show("Error Roles: " + ex.Message); }
        }

        private void CargarUsuarios()
        {
            try
            {
                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = @"SELECT u.idUsuario, u.Username, r.NombreRol 
                                     FROM Usuarios u
                                     JOIN Roles r ON u.idRol = r.idRol";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgUsuarios.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Usuarios: " + ex.Message); }
        }

        // --- GUARDAR CON ENCRIPTACIÓN ---
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password) || cbRol.SelectedValue == null)
            {
                MessageBox.Show("Completa todos los datos.");
                return;
            }

            try
            {
                // 1. Encriptar contraseña antes de enviarla
                string passEncriptada = GetSHA256(txtPassword.Password);

                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    string query = "INSERT INTO Usuarios (Username, PasswordHash, idRol) VALUES (@user, @pass, @rol)";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", passEncriptada); // Guardamos la encriptada
                    cmd.Parameters.AddWithValue("@rol", cbRol.SelectedValue);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario creado exitosamente.");

                    txtUsername.Clear();
                    txtPassword.Clear();
                    CargarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error (probablemente el usuario ya existe): " + ex.Message);
            }
        }

        // --- MÉTODO DE ENCRIPTACIÓN (Helper) ---
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
        // Pega esto dentro de tu clase GestionUsuarios, antes de la última llave
        private void cbRol_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Este método se creó por accidente al dar doble clic, lo dejamos vacío para que no marque error.
        }
    }

}