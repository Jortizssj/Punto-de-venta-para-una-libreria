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
        private int _idEmpleadoSeleccionado = 0;
        public Empleados()
        {
            InitializeComponent();
            _empleadoDAO = new EmpleadoDAO();
            CargarDatosEnCuadricula();
        }

        private void CargarDatosEnCuadricula()
        {
            try
            {
                // Uso del DAO para obtener datos via Stored Procedure
                dataGridViewEmpleados.DataSource = _empleadoDAO.ObtenerTodosEmpleados();

                // Ocultar la columna de ID al usuario
                if (dataGridViewEmpleados.Columns.Contains("ID_Empleado"))
                {
                    dataGridViewEmpleados.Columns["ID_Empleado"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES (TRY-CATCH-FINALLY)
                MessageBox.Show("Error al cargar los empleados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) || cmbCargo.SelectedIndex == -1)
            {
                MessageBox.Show("Debe completar todos los campos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. CREAR OBJETO POJO
            EmpleadoPOJO empleado = new EmpleadoPOJO
            {
                ID_Empleado = _idEmpleadoSeleccionado,
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Cargo = cmbCargo.SelectedItem.ToString()
            };

            // 3. LLAMADA A LA CAPA DE DATOS (Usando Stored Procedures)
            try
            {
                bool resultado;
                string mensaje;

                if (_idEmpleadoSeleccionado > 0)
                {
                    // Actualizar (usa sp_ActualizarEmpleado)
                    resultado = _empleadoDAO.ActualizarEmpleado(empleado);
                    mensaje = "Empleado actualizado exitosamente.";
                }
                else
                {
                    // Insertar (usa sp_InsertarEmpleado)
                    resultado = _empleadoDAO.InsertarEmpleado(empleado);
                    mensaje = "Empleado guardado exitosamente.";
                }

                if (resultado)
                {
                    MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatosEnCuadricula();
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al guardar el empleado.", "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES
                MessageBox.Show("Error inesperado: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idEmpleadoSeleccionado > 0)
            {
                if (MessageBox.Show("¿Está seguro de que desea eliminar lógicamente a este empleado?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // Eliminar lógicamente (usa sp_EliminarEmpleado)
                        if (_empleadoDAO.EliminarEmpleado(_idEmpleadoSeleccionado))
                        {
                            MessageBox.Show("Empleado eliminado lógicamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarFormulario();
                            CargarDatosEnCuadricula();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar al empleado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al intentar eliminar: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado de la lista para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }
        private void LimpiarFormulario()
        {
            _idEmpleadoSeleccionado = 0;
            txtNombre.Clear();
            txtApellido.Clear();
            cmbCargo.SelectedIndex = 0;
            btnGuardar.Text = "Guardar";
            txtNombre.Focus();
        }
    }
}
