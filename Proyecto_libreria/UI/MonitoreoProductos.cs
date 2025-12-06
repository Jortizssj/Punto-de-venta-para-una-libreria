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
    public partial class MonitoreoProductos : Form
    {
        public MonitoreoProductos()
        {
            InitializeComponent();
            CargarAuditoria();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuditoria)).BeginInit();
        }

        private void CargarAuditoria()
        {
            try
            {
                // Llamada a la Capa de Datos para obtener el historial
                AuditoriaDAO aud = new AuditoriaDAO();
                DataTable auditoria = aud.ObtenerHistorialAuditoria();

                dataGridViewAuditoria.DataSource = auditoria;

                if (auditoria.Rows.Count > 0)
                {
                    // Configuración visual de las columnas
                    dataGridViewAuditoria.Columns["Fecha_Hora"].HeaderText = "Fecha y Hora";
                    dataGridViewAuditoria.Columns["Tipo_Cambio"].HeaderText = "Tipo de Cambio";
                    dataGridViewAuditoria.Columns["Usuario_DB"].HeaderText = "Usuario BD";
                    dataGridViewAuditoria.Columns["ISBN"].HeaderText = "CLAVE (ISBN)";
                    dataGridViewAuditoria.Columns["Campo_Afectado"].HeaderText = "Campo Afectado";
                    dataGridViewAuditoria.Columns["Valor_Anterior"].HeaderText = "Valor Anterior";
                    dataGridViewAuditoria.Columns["Valor_Nuevo"].HeaderText = "Valor Nuevo";

                    // Ajustar el ancho de las columnas
                    dataGridViewAuditoria.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
                else
                {
                    MessageBox.Show("No se encontraron registros de auditoría.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES
                MessageBox.Show("Error al cargar el historial de auditoría: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarAuditoria();
        }

        private void dataGridViewAuditoria_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
