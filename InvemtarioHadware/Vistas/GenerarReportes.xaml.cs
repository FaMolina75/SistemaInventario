using System;
using System.Data;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using InvemtarioHadware.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace InvemtarioHadware.Vistas
{
    public partial class GenerarReportes : Window
    {
        private int rolUsuario;

        // ESTE ES EL CONSTRUCTOR QUE RECIBE EL ROL
        public GenerarReportes(int rol)
        {
            InitializeComponent();
            this.rolUsuario = rol;
            AplicarPermisos();
        }

        // Constructor vacío por seguridad
        public GenerarReportes() : this(0) { }

        private void AplicarPermisos()
        {
            // Si no es Admin, bloqueamos el reporte completo
            if (rolUsuario != 1)
            {
                rbInventario.IsEnabled = false;
                rbInventario.IsChecked = false;
                rbAsignaciones.IsChecked = true;
                lblRestriccion.Visibility = Visibility.Visible;
            }
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            string titulo = "", query = "", archivo = "";

            if (rbInventario.IsChecked == true)
            {
                titulo = "Inventario General";
                archivo = "Inventario";
                query = @"SELECT h.Serie, m.NombreModelo, u.NombreUbicacion, h.Estado 
                          FROM Hardware h
                          JOIN Modelos m ON h.idModelo = m.idModelo
                          JOIN Ubicaciones u ON h.idUbicacion = u.idUbicacion";
            }
            else if (rbAsignaciones.IsChecked == true)
            {
                titulo = "Reporte de Asignaciones";
                archivo = "Asignaciones";
                query = @"SELECT a.FechaAsignacion, h.Serie, e.Nombre 
                          FROM AsignacionesHW a
                          JOIN Hardware h ON a.idHardware = h.idHardware
                          JOIN Empleados e ON a.idEmpleado = e.idEmpleado";
            }
            else if (rbMantenimientos.IsChecked == true)
            {
                titulo = "Mantenimientos";
                archivo = "Mantenimientos";
                query = @"SELECT m.Fecha, h.Serie, p.NombreProveedor, m.Costo 
                          FROM MantenimientosHW m
                          JOIN Hardware h ON m.idHardware = h.idHardware
                          JOIN Proveedores p ON m.idProveedor = p.idProveedor";
            }

            CrearPDF(titulo, archivo, query);
        }

        private void CrearPDF(string titulo, string nombreArch, string query)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF (*.pdf)|*.pdf";
            sfd.FileName = nombreArch + "_" + DateTime.Now.ToString("dd-MM-yyyy");

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    DataTable dt = new DataTable();
                    ConexionDB db = new ConexionDB();
                    using (MySqlConnection con = db.GetConnection())
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                        da.Fill(dt);
                    }

                    Document doc = new Document(PageSize.LETTER.Rotate());
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    doc.Add(new Paragraph(titulo, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
                    doc.Add(new Paragraph("Generado el: " + DateTime.Now.ToString()));
                    doc.Add(new Paragraph(" "));

                    PdfPTable pdfTable = new PdfPTable(dt.Columns.Count);
                    pdfTable.WidthPercentage = 100;

                    foreach (DataColumn col in dt.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName));
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        pdfTable.AddCell(cell);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (var item in row.ItemArray)
                        {
                            pdfTable.AddCell(item.ToString());
                        }
                    }

                    doc.Add(pdfTable);
                    doc.Close();
                    MessageBox.Show("Reporte generado.");
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}