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
    public partial class Empleados : Form
    {
        private EmpleadoDAO _empleadoDAO = new EmpleadoDAO();
        private UsuarioDAO _usuarioDAO = new UsuarioDAO();

        private int _idEmpleadoEdicion = 0;

        public Empleados()
        {
            InitializeComponent();
            ConfigurarControles();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                DataTable dt = _empleadoDAO.ObtenerTodosEmpleados();

                dataGridViewEmpleados.DataSource = dt;

                if (dataGridViewEmpleados.Columns.Contains("ID_Empleado"))
                    dataGridViewEmpleados.Columns["ID_Empleado"].Visible = false ;

                if (dataGridViewEmpleados.Columns.Contains("Activo"))
                    dataGridViewEmpleados.Columns["Activo"].Visible = false;

                if (dataGridViewEmpleados.Columns.Contains("Contrasena_Hash"))
                    dataGridViewEmpleados.Columns["Contrasena_Hash"].Visible = false;

                dataGridViewEmpleados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar lista: " + ex.Message);
            }
        }

        private void ConfigurarControles()
        {
            dataGridViewEmpleados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewEmpleados.MultiSelect = false;
            dataGridViewEmpleados.ReadOnly = true;

            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("Administrador");
            cmbTipo.Items.Add("Vendedor");
            cmbTipo.SelectedIndex = 0;

            LimpiarFormulario();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                MessageBox.Show("Debe llenar: Nombre, Apellidos, Usuario y Contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                EmpleadoPOJO nuevoEmpleado = new EmpleadoPOJO
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Cargo = txtCargo.Text.Trim(),
                    Activo = true
                };

                int idGenerado = _empleadoDAO.InsertarEmpleado(nuevoEmpleado);

                if (idGenerado > 0)
                {
                    // 2. Insertar Usuario vinculado
                    UsuarioPOJO nuevoUsuario = new UsuarioPOJO
                    {
                        NombreUsuario = txtUsuario.Text.Trim(),
                        Tipo_Usuario = cmbTipo.SelectedItem.ToString(),
                        ID_Empleado = idGenerado
                    };

                    if (_usuarioDAO.RegistrarUsuario(nuevoUsuario, txtContrasenia.Text.Trim()))
                    {
                        MessageBox.Show("Empleado registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show("Empleado creado, pero el usuario ya existe o falló.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el empleado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (_idEmpleadoEdicion == 0) return;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Nombre y Apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                EmpleadoPOJO empActualizado = new EmpleadoPOJO
                {
                    ID_Empleado = _idEmpleadoEdicion,
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Cargo = txtCargo.Text.Trim(),
                    Activo = true
                };

                if (_empleadoDAO.ActualizarEmpleado(empActualizado))
                {
                    MessageBox.Show("Datos actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatos();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void LimpiarFormulario()
        {
            _idEmpleadoEdicion = 0;
            txtNombre.Clear();
            txtApellido.Clear();
            txtCargo.Clear();
            txtUsuario.Clear();
            txtContrasenia.Clear();

            cmbTipo.SelectedIndex = 0;

            txtUsuario.Enabled = true;
            txtContrasenia.Enabled = true;
            cmbTipo.Enabled = true;

            btnAgregar.Enabled = true;
            btnEliminar.Enabled = true;
            btnGuardarCambios.Enabled = false;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmpleados.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un empleado del grid.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow row = dataGridViewEmpleados.CurrentRow;

                if (row.Cells["ID_Empleado"].Value == null) return;

                _idEmpleadoEdicion = Convert.ToInt32(row.Cells["ID_Empleado"].Value);

                // Cargar TextBoxes
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
                txtApellido.Text = row.Cells["Apellido"].Value?.ToString() ?? "";
                txtCargo.Text = row.Cells["Cargo"].Value?.ToString() ?? "";

                // Cargar Usuario/Rol
                if (dataGridViewEmpleados.Columns.Contains("NombreUsuario"))
                    txtUsuario.Text = row.Cells["NombreUsuario"].Value?.ToString();

                if (dataGridViewEmpleados.Columns.Contains("Tipo_Usuario"))
                {
                    string tipo = row.Cells["Tipo_Usuario"].Value?.ToString();
                    if (cmbTipo.Items.Contains(tipo)) cmbTipo.SelectedItem = tipo;
                }

                txtUsuario.Enabled = false;
                txtContrasenia.Enabled = false;
                cmbTipo.Enabled = false;
                txtContrasenia.Text = "";

                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;
                btnGuardarCambios.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
                LimpiarFormulario();
            }
        }

        private void btnGuardarCambios_Click_1(object sender, EventArgs e)
        {
            if (_idEmpleadoEdicion == 0) return;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Nombre y Apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                EmpleadoPOJO empActualizado = new EmpleadoPOJO
                {
                    ID_Empleado = _idEmpleadoEdicion,
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Cargo = txtCargo.Text.Trim(),
                    Activo = true
                };

                if (_empleadoDAO.ActualizarEmpleado(empActualizado))
                {
                    MessageBox.Show("Datos actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatos();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmpleados.CurrentRow == null)
            {
                MessageBox.Show("Seleccione el empleado que desea eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombre = dataGridViewEmpleados.CurrentRow.Cells["Nombre"].Value.ToString();
            if (dataGridViewEmpleados.CurrentRow.Cells["ID_Empleado"].Value == null) return;
            int idEmp = Convert.ToInt32(dataGridViewEmpleados.CurrentRow.Cells["ID_Empleado"].Value);

            DialogResult result = MessageBox.Show($"¿Está seguro de eliminar a '{nombre}'? (Borrado Lógico)", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_empleadoDAO.EliminarEmpleado(idEmp))
                    {
                        MessageBox.Show("Empleado eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el empleado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }

        }
    }
}
