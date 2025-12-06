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
            this.AcceptButton = btnIngresar;

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
            try
            {
                UsuarioPOJO usuarioAutenticado = UsuarioDAO.ValidarCredenciales(usuario, password);

                if (usuarioAutenticado != null)
                {
                    MessageBox.Show($"Bienvenido(a), {usuarioAutenticado.NombreUsuario}! Tu rol es: {usuarioAutenticado.Tipo_Usuario}.", "Acceso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();

                    Dashboard dashboard = new Dashboard(usuarioAutenticado);
                    dashboard.ShowDialog();

                    this.Show();
                    txtPassword.Clear();
                    txtUsuario.Clear();
                    txtUsuario.Focus();
                }
                else
                {
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
