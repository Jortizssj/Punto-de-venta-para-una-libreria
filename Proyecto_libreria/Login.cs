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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            ConfigurarUsabilidad();
        }
        private void ConfigurarUsabilidad()
        {
            // Permitir que Enter en la contraseña active el botón Ingresar
            this.AcceptButton = btnIngresar;

            // Ocultar la contraseña
            txtPassword.UseSystemPasswordChar = true;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Debe ingresar usuario y contraseña.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. LLAMADA A LA CAPA DE DATOS (No hay SQL aquí)
            try
            {
                UsuarioPOJO usuarioAutenticado = UsuarioDAO.ValidarCredenciales(usuario, password);

                if (usuarioAutenticado != null)
                {
                    MessageBox.Show($"Bienvenido(a), {usuarioAutenticado.NombreUsuario}! Tu rol es: {usuarioAutenticado.Tipo_Usuario}.", "Acceso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abrir el formulario principal (Dashboard)
                    this.Hide();

                    // 2. Crear y mostrar el Dashboard, PASANDO el objeto Usuario
                    Dashboard dashboard = new Dashboard(usuarioAutenticado);
                    dashboard.ShowDialog();

                    this.Show();
                    txtPassword.Clear();
                    txtUsuario.Clear();
                    txtUsuario.Focus();
                }
                else
                {
                    // ACCESO DENEGADO
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error del sistema: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
