using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto_libreria;

namespace Proyecto_libreria
{
    /// <summary>
    /// Formulario principal o Dashboard del sistema.
    /// Controla el acceso a las diferentes funcionalidades según el rol del usuario.
    /// </summary>
    public partial class Dashboard : Form
    {
        private UsuarioPOJO _usuarioAutenticado;
        public Dashboard(UsuarioPOJO usuario)
        {
            InitializeComponent();
            _usuarioAutenticado = usuario;
            this.Text = $"Sistema POS - Bienvenido(a) {_usuarioAutenticado.NombreUsuario}";

            // Configurar el acceso basado en el rol al cargar
            CargarPermisos();
        }

        /// <summary>
        /// Oculta o muestra elementos del menú según el Tipo_Usuario.
        /// </summary>
        private void CargarPermisos()
        {
            bool esAdmin = (_usuarioAutenticado.Tipo_Usuario == "Administrador");

            // Elementos disponibles para VENDEDOR y ADMINISTRADOR
            btnPuntoVenta.Visible = true;
            lblRol.Text = $"Rol: {_usuarioAutenticado.Tipo_Usuario}";

            // Reportes: Todos pueden verlos, pero el CRUD completo es de Admin.
            btnVentasVendedor.Visible = true;
            btnUnidadesVendidas.Visible = true;

            // Elementos exclusivos del ADMINISTRADOR
            btnCRUD_Empleados.Visible = esAdmin;
            btnMonitoreo.Visible = esAdmin;
            btnCRUD_Productos.Visible = esAdmin;
        }

        // Método genérico para abrir formularios
        private void AbrirFormulario(Form formulario)
        {
            // Uso de TRY-CATCH para Control de Errores (REQUISITO DE COTEJO)
            try
            {
                // Usamos ShowDialog para que el usuario deba cerrar la ventana secundaria antes de volver al menú.
                formulario.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ventana: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Eventos de Botones de Funcionalidad Principal ---

        private void btnPuntoVenta_Click(object sender, EventArgs e)
        {
            // Se debe pasar el usuario al Punto de Venta para registrar la transacción
            // Asumiendo que tu archivo de PuntoVenta es 'PuntoVenta.cs'
            AbrirFormulario(new PuntoVenta(_usuarioAutenticado));
        }

        private void btnCRUD_Productos_Click(object sender, EventArgs e)
        {
            // Asumiendo que tu archivo de Productos es 'Productos.cs'
            AbrirFormulario(new Productos());
        }

        private void btnCRUD_Empleados_Click(object sender, EventArgs e)
        {
            // Asumiendo que tu archivo de Empleados es 'Empleados.cs'
            AbrirFormulario(new Empleados());
        }

        // --- Eventos de Botones de Reportes ---
        private void btnVentasVendedor_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new ReporteVentasVendedor());
        }

        private void btnUnidadesVendidas_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new ReporteUnidadesVendidas());
        }

        private void btnMonitoreo_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new MonitoreoProductos());
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sesión cerrada. Gracias por usar el sistema.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
    }
}
