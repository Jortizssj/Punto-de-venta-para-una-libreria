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
    /// <summary>
    /// Formulario para generar el Reporte de Ventas por Empleado en un período.
    /// </summary>
    public partial class ReporteVentasVendedor : Form
    {
        public ReporteVentasVendedor()
        {
            InitializeComponent();
            ConfigurarControles();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReporte)).BeginInit();
        }
        private void ConfigurarControles()
        {
            // USABILIDAD: Usar DateTimePicker para las fechas
            dtpFechaInicio.Format = DateTimePickerFormat.Short;
            dtpFechaFin.Format = DateTimePickerFormat.Short;

            // Establecer un rango de fechas por defecto (ej. el mes actual)
            dtpFechaFin.Value = DateTime.Today;
            dtpFechaInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // Configuración inicial de la cuadrícula
            dataGridViewReporte.AutoGenerateColumns = true;
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            // VALIDACIÓN: Asegurar que la fecha de inicio no sea posterior a la fecha de fin
            if (dtpFechaInicio.Value.Date > dtpFechaFin.Value.Date)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Llamada a la Capa de Datos
                ReporteDAO dao = new ReporteDAO();
                DataTable reporte = dao.ObtenerVentasPorVendedor(dtpFechaInicio.Value.Date, dtpFechaFin.Value.Date);

                dataGridViewReporte.DataSource = reporte;

                if (reporte.Rows.Count > 0)
                {
                    // Formatear la columna de MONTO_VENDIDO a moneda
                    if (dataGridViewReporte.Columns.Contains("MONTO_VENDIDO"))
                    {
                        dataGridViewReporte.Columns["MONTO_VENDIDO"].HeaderText = "MONTO VENDIDO";
                        dataGridViewReporte.Columns["MONTO_VENDIDO"].DefaultCellStyle.Format = "C2"; // Formato de moneda
                    }
                }
                else
                {
                    MessageBox.Show("No se encontraron ventas para el período seleccionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
