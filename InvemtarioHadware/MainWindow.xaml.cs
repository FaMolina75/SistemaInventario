using System;
using System.Windows;
using InvemtarioHadware.Vistas;

namespace InvemtarioHadware
{
    public partial class MainWindow : Window
    {
        private int rolUsuario;

        // Constructor PRINCIPAL
        public MainWindow(int rol)
        {
            InitializeComponent();
            this.rolUsuario = rol;
            AplicarPermisos();
        }

        // Constructor vacío de seguridad
        public MainWindow() : this(0) { }

        private void AplicarPermisos()
        {
            if (btnUsuarios != null)
            {
                // Si no es Admin (1), ocultar Usuarios
                if (rolUsuario != 1)
                {
                    btnUsuarios.Visibility = Visibility.Collapsed;
                }
                else
                {
                    btnUsuarios.Visibility = Visibility.Visible;
                }
            }
        }

        // --- EVENTO DE CERRAR SESIÓN (NUEVO) ---
        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            // 1. Crear nueva ventana de Login
            Login loginWindow = new Login();
            loginWindow.Show();

            // 2. Cerrar el Dashboard actual
            this.Close();
        }

        // --- TUS BOTONES DE MÓDULOS ---

        private void Button_Hardware_Click(object sender, RoutedEventArgs e)
        {
            GestionHardware ventana = new GestionHardware();
            ventana.Show();
        }

        private void Button_Mantenimiento_Click(object sender, RoutedEventArgs e)
        {
            HistorialMantenimiento ventana = new HistorialMantenimiento();
            ventana.Show();
        }

        private void Button_Asignaciones_Click(object sender, RoutedEventArgs e)
        {
            ControlAsignaciones ventana = new ControlAsignaciones();
            ventana.Show();
        }

        private void Button_Catalogos_Click(object sender, RoutedEventArgs e)
        {
            GestionCatalogos ventana = new GestionCatalogos();
            ventana.Show();
        }

        private void Button_Ubicaciones_Click(object sender, RoutedEventArgs e)
        {
            GestionUbicaciones ventana = new GestionUbicaciones();
            ventana.Show();
        }

        private void Button_Usuarios_Click(object sender, RoutedEventArgs e)
        {
            GestionUsuarios ventana = new GestionUsuarios();
            ventana.Show();
        }

        private void Button_Reportes_Click(object sender, RoutedEventArgs e)
        {
            GenerarReportes ventana = new GenerarReportes(this.rolUsuario);
            ventana.Show();
        }
    }
}