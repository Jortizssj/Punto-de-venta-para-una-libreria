using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_libreria
{
    public partial class ReporteUnidadesVendidas : Form
    {
        public ReporteUnidadesVendidas()
        {
            InitializeComponent();

            // VALIDACIÓN: Asegurarse de que el año solo contenga números
            this.txtAnio.KeyPress += new KeyPressEventHandler((sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            });

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReporte)).BeginInit();
        }
        private void ConfigurarControles()
        {
            txtAnio.Text = DateTime.Today.Year.ToString();
            cmbMes.SelectedIndex = DateTime.Today.Month - 1; // Mes actual por defecto

            // Configuración inicial de la cuadrícula
            dataGridViewReporte.AutoGenerateColumns = true;
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            // VALIDACIÓN: Asegurar que se haya seleccionado mes y el año sea numérico
            if (cmbMes.SelectedIndex == -1 || !int.TryParse(txtAnio.Text, out int anio) || anio < 1900 || anio > DateTime.Today.Year)
            {
                MessageBox.Show("Seleccione un mes válido e ingrese un año correcto (solo números).", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // El índice del mes en C# es 0-11, pero la función MONTH() de MySQL es 1-12
            int mesMySQL = cmbMes.SelectedIndex + 1;

            try
            {
                // Llamada a la Capa de Datos
                ReporteDAO dao = new ReporteDAO();
                DataTable reporte = dao.ObtenerUnidadesVendidasPorLibro(mesMySQL, anio);

                dataGridViewReporte.DataSource = reporte;

                if (reporte.Rows.Count > 0)
                {
                    // Formatear las columnas
                    if (dataGridViewReporte.Columns.Contains("COSTO"))
                    {
                        dataGridViewReporte.Columns["COSTO"].DefaultCellStyle.Format = "C2";
                    }
                    if (dataGridViewReporte.Columns.Contains("TITULO"))
                    {
                        dataGridViewReporte.Columns["TITULO"].HeaderText = "NOMBRE";
                    }
                    if (dataGridViewReporte.Columns.Contains("UNIDADES_VENDIDAS"))
                    {
                        dataGridViewReporte.Columns["UNIDADES_VENDIDAS"].HeaderText = "UNIDADES VENDIDAS";
                    }
                }
                else
                {
                    MessageBox.Show($"No se encontraron ventas de libros en {cmbMes.SelectedItem} de {anio}.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES
                MessageBox.Show("Error al generar el reporte: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
