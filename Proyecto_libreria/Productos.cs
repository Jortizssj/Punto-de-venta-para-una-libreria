using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace Proyecto_libreria
{
    /// <summary>
    /// Formulario para la gestión (CRUD) de Productos (libros).
    /// </summary>
    public partial class Productos : Form
    {
        private readonly ProductoDAO _productoDAO = new ProductoDAO();

        public Productos()
        {
            InitializeComponent();
            CargarDatosEnCuadricula();
            ConfigurarUsabilidad();

            // Vincular evento de selección
            this.dataGridViewProductos.SelectionChanged += DataGridViewProductos_SelectionChanged;
        }

        private void CargarDatosEnCuadricula()
        {
            try
            {
                dataGridViewProductos.DataSource = _productoDAO.ObtenerTodosProductos();

                if (dataGridViewProductos.Columns.Contains("PRECIO"))
                    dataGridViewProductos.Columns["PRECIO"].DefaultCellStyle.Format = "C2";

                if (dataGridViewProductos.Columns.Contains("ISBN"))
                    dataGridViewProductos.Columns["ISBN"].HeaderText = "CLAVE (ISBN)";

                dataGridViewProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void ConfigurarUsabilidad()
        {
            dataGridViewProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewProductos.MultiSelect = false;
            dataGridViewProductos.ReadOnly = true;

            LimpiarFormulario();
        }
        private void DataGridViewProductos_SelectionChanged(object sender, EventArgs e)
        {
            GenerateBarcodeWithLibrary();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            ProductoPOJO producto = CrearObjetoDesdeInterfaz();

            try
            {
                if (_productoDAO.BuscarProductoPorISBN(producto.ClaveISBN) != null)
                {
                    MessageBox.Show("El ISBN ya existe.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_productoDAO.AgregarProducto(producto))
                {
                    MessageBox.Show("Producto agregado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatosEnCuadricula();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridViewProductos.CurrentRow == null) return;

            DataGridViewRow fila = dataGridViewProductos.CurrentRow;

            txtISBN.Text = fila.Cells["ISBN"].Value.ToString();
            txtNombre.Text = fila.Cells["NOMBRE"].Value.ToString();
            txtDescripcion.Text = fila.Cells["DESCRIPCION"].Value.ToString();
            txtPrecio.Text = fila.Cells["PRECIO"].Value.ToString();

            if (int.TryParse(fila.Cells["STOCK"].Value.ToString(), out int stock))
                numStock.Value = stock;

            txtISBN.Enabled = false; // Bloquear clave primaria
            MessageBox.Show("Datos cargados. Edite y guarde.", "Edición", MessageBoxButtons.OK, MessageBoxIcon.Information);

            GenerateBarcodeWithLibrary(); // Actualizar visualmente
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text)) return;
            if (!ValidarCampos()) return;

            try
            {
                if (_productoDAO.ActualizarProducto(CrearObjetoDesdeInterfaz()))
                {
                    MessageBox.Show("Producto actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatosEnCuadricula();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewProductos.CurrentRow == null) return;

            string isbn = dataGridViewProductos.CurrentRow.Cells["ISBN"].Value.ToString();
            string nombre = dataGridViewProductos.CurrentRow.Cells["NOMBRE"].Value.ToString();

            if (MessageBox.Show($"¿Eliminar '{nombre}'?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    if (_productoDAO.EliminarProducto(isbn))
                    {
                        MessageBox.Show("Eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarDatosEnCuadricula();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar (¿Tiene ventas?).");
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void LimpiarFormulario()
        {
            txtISBN.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            numStock.Value = 0;
            txtISBN.Enabled = true;

            if (pictureBoxBarras.Image != null)
            {
                pictureBoxBarras.Image.Dispose();
                pictureBoxBarras.Image = null;
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Campos obligatorios vacíos.");
                return false;
            }
            return true;
        }

        private ProductoPOJO CrearObjetoDesdeInterfaz()
        {
            decimal.TryParse(txtPrecio.Text, out decimal precio);
            return new ProductoPOJO
            {
                ClaveISBN = txtISBN.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim(),
                Precio = precio,
                Stock = (int)numStock.Value
            };
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }

        private void GenerateBarcodeWithLibrary()
        {
            // 1. Limpieza previa
            if (pictureBoxBarras.Image != null)
            {
                pictureBoxBarras.Image.Dispose();
                pictureBoxBarras.Image = null;
            }

            // 2. Validaciones de selección
            if (dataGridViewProductos.CurrentRow == null) return;
            var cell = dataGridViewProductos.CurrentRow.Cells["ISBN"];
            if (cell == null || cell.Value == null) return;

            string codigo = cell.Value.ToString().Trim();
            if (string.IsNullOrEmpty(codigo)) return;

            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = pictureBoxBarras.Height,
                        Width = pictureBoxBarras.Width,
                        PureBarcode = false,
                        Margin = 1
                    }
                };

                // 4. Lógica inteligente para ISBN (Si son puros números y largo 13, usa EAN_13)
                // Esto hace que los libros se vean con el formato estándar de librería.
                bool soloNumeros = codigo.All(char.IsDigit);
                if (soloNumeros && (codigo.Length == 12 || codigo.Length == 13))
                {
                    writer.Format = BarcodeFormat.EAN_13;
                }

                pictureBoxBarras.Image = writer.Write(codigo);
            }
            catch (Exception)
            {
                // Si falla (ej. formato inválido para EAN-13), intentamos fallback a Code 128
                try
                {
                    var writerFallback = new BarcodeWriter
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new ZXing.Common.EncodingOptions { Height = pictureBoxBarras.Height, Width = pictureBoxBarras.Width }
                    };
                    pictureBoxBarras.Image = writerFallback.Write(codigo);
                }
                catch { /* Si falla todo, simplemente no mostramos nada */ }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text)) return;
            if (!ValidarCampos()) return;

            try
            {
                if (_productoDAO.ActualizarProducto(CrearObjetoDesdeInterfaz()))
                {
                    MessageBox.Show("Producto actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatosEnCuadricula();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Productos_Load(object sender, EventArgs e)
        {

        }
    }
}
