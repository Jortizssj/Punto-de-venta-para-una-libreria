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
    /// Formulario para la gestión (CRUD) de Productos (libros).
    /// </summary>
    public partial class Productos : Form
    {
        // Utiliza la variable DAO sin readonly para ser flexible, aunque el constructor es preferido
        private ProductoDAO _productoDAO = new ProductoDAO();
        private bool _isEditing = false;
        private string _isbnSeleccionado = string.Empty;
        public Productos()
        {
            InitializeComponent();
            _productoDAO = new ProductoDAO();
            CargarDatosEnCuadricula();
        }
        private void CargarDatosEnCuadricula()
        {
            try
            {
                // Llamada a la Capa de Datos (DAO)
                dataGridViewProductos.DataSource = _productoDAO.ObtenerTodosProductos();

                // Configuración visual
                if (dataGridViewProductos.Columns.Contains("PRECIO"))
                {
                    dataGridViewProductos.Columns["PRECIO"].DefaultCellStyle.Format = "C2";
                }
                if (dataGridViewProductos.Columns.Contains("ISBN"))
                {
                    dataGridViewProductos.Columns["ISBN"].HeaderText = "CLAVE (ISBN)";
                }
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES (TRY-CATCH-FINALLY)
                MessageBox.Show("Error al cargar los productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Por favor, complete los campos CLAVE, Nombre y Precio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio debe ser un valor numérico válido.", "Validación de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_isbnSeleccionado))
            {
                MessageBox.Show("Seleccione un producto de la lista para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"¿Está seguro de eliminar el producto con ISBN: {_isbnSeleccionado}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_productoDAO.EliminarProducto(_isbnSeleccionado))
                    {
                        MessageBox.Show("Producto eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatosEnCuadricula();
                    }
                }
                catch (Exception ex)
                {
                    // Manejo del error de clave foránea desde el DAO
                    MessageBox.Show("Error al intentar eliminar: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridViewProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (string.IsNullOrEmpty(_isbnSeleccionado))
            {
                MessageBox.Show("Seleccione un producto de la lista para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"¿Está seguro de eliminar el producto con ISBN: {_isbnSeleccionado}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_productoDAO.EliminarProducto(_isbnSeleccionado))
                    {
                        MessageBox.Show("Producto eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatosEnCuadricula();
                    }
                }
                catch (Exception ex)
                {
                    // Manejo del error de clave foránea desde el DAO
                    MessageBox.Show("Error al intentar eliminar: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }
        private void LimpiarFormulario()
        {
            _isbnSeleccionado = string.Empty;
            _isEditing = false;
            txtISBN.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            numStock.Value = 0;
            btnGuardar.Text = "Guardar";
            txtISBN.Enabled = true; // Habilitar ISBN para nuevo registro
            txtISBN.Focus();
        }
    }
}
