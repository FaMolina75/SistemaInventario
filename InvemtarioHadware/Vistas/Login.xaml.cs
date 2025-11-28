using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;

namespace InvemtarioHadware.Vistas
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPass.Password;

            // Validación básica de campos vacíos
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Ingresa usuario y contraseña.");
                return;
            }

            try
            {
                // 1. Encriptamos la contraseña para compararla
                string passHash = GestionUsuarios.GetSHA256(pass);

                ConexionDB db = new ConexionDB();
                using (MySqlConnection con = db.GetConnection())
                {
                    // 2. Buscamos si existe el usuario y pedimos su ROL
                    string query = "SELECT idRol FROM Usuarios WHERE Username=@u AND PasswordHash=@p";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", passHash);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        // LOGIN EXITOSO
                        int rolEncontrado = Convert.ToInt32(resultado);

                        // Abrimos el Dashboard enviando el rol (silenciosamente)
                        MainWindow dashboard = new MainWindow(rolEncontrado);
                        dashboard.Show();

                        // Cerramos la ventana de Login
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}