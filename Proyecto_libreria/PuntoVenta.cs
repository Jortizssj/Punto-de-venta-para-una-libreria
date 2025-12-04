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
    public partial class PuntoVenta : Form
    {
        private readonly ProductoDAO _productoDAO = new ProductoDAO();
        private readonly VentaDAO _ventaDAO = new VentaDAO();
        private List<DetalleVentaPOJO> _carrito = new List<DetalleVentaPOJO>();
        private UsuarioPOJO _vendedorActual; // Usuario que inició sesión

        public PuntoVenta(UsuarioPOJO vendedor)
        {
            InitializeComponent();

            if (vendedor != null)
            {
                _vendedorActual = vendedor;

                if (_vendedorActual.ID_Empleado > 0)
                {
                    lblVendedor.Text = $"Vendedor: {_vendedorActual.NombreUsuario}"; 
                }
                else
                {
                    // Si el usuario existe pero no tiene ID_Empleado asignado, es un error de configuración de BD
                    MessageBox.Show("Advertencia: El usuario autenticado no tiene un ID de empleado válido (> 0) para registrar ventas.", "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Error: El formulario de Venta no recibió el usuario autenticado.", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ConfigurarCuadriculaCarrito();
            ConfigurarUsabilidad();

        }

        private void ConfigurarCuadriculaCarrito()
        {
            dataGridViewCarrito.AutoGenerateColumns = false;
            dataGridViewCarrito.Columns.Clear();

            // Definición manual de columnas para mejor control visual
            dataGridViewCarrito.Columns.Add("Nombre", "Libro");
            dataGridViewCarrito.Columns["Nombre"].DataPropertyName = "NombreProducto";

            dataGridViewCarrito.Columns.Add("Cantidad", "Cantidad");
            dataGridViewCarrito.Columns["Cantidad"].DataPropertyName = "Cantidad";

            dataGridViewCarrito.Columns.Add("Precio", "Precio Unitario");
            dataGridViewCarrito.Columns["Precio"].DataPropertyName = "PrecioUnitario";
            dataGridViewCarrito.Columns["Precio"].DefaultCellStyle.Format = "C2";

            dataGridViewCarrito.Columns.Add("Subtotal", "Subtotal");
            dataGridViewCarrito.Columns["Subtotal"].DataPropertyName = "Subtotal";
            dataGridViewCarrito.Columns["Subtotal"].DefaultCellStyle.Format = "C2";

            // Ocultar la CLAVE, es solo para referencia interna
            dataGridViewCarrito.Columns.Add("ClaveProducto", "CLAVE");
            dataGridViewCarrito.Columns["ClaveProducto"].DataPropertyName = "ClaveProducto";
            dataGridViewCarrito.Columns["ClaveProducto"].Visible = false;
        }

        private void ConfigurarUsabilidad()
        {
            txtISBN.TabIndex = 0;
            btnAgregar.TabIndex = 1;
            btnCobrar.TabIndex = 2;

            txtISBN.KeyPress += new KeyPressEventHandler(txtISBN_KeyPress);
        }

        // Evento que se dispara al presionar Enter en el campo ISBN (Simula escaneo)
        private void txtISBN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnAgregar_Click(sender, e);
            }
        }

        // Simula la adición de un producto al escanear el ISBN
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string isbn = txtISBN.Text.Trim();
            int cantidad = (int)numCantidad.Value; // Asumimos un NumericUpDown para la cantidad

            if (string.IsNullOrEmpty(isbn) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese un ISBN válido y una cantidad mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Buscar producto en la BD (Capa de Datos) usando la instancia del DAO
                ProductoPOJO producto = _productoDAO.BuscarProductoPorISBN(isbn);

                if (producto == null)
                {
                    MessageBox.Show($"Producto con ISBN {isbn} no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Verificar STOCK (Validación)
                int cantidadEnCarrito = _carrito.Where(d => d.ClaveProducto == isbn).Sum(d => d.Cantidad);
                if (producto.Stock < (cantidadEnCarrito + cantidad))
                {
                    MessageBox.Show($"Stock insuficiente. Solo quedan {producto.Stock - cantidadEnCarrito} unidades disponibles.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Agregar o actualizar en el carrito
                var itemExistente = _carrito.FirstOrDefault(d => d.ClaveProducto == isbn);

                if (itemExistente != null)
                {
                    itemExistente.Cantidad += cantidad;
                }
                else
                {
                    _carrito.Add(new DetalleVentaPOJO
                    {
                        ClaveProducto = producto.ClaveISBN,
                        NombreProducto = producto.Nombre,
                        Cantidad = cantidad,
                        PrecioUnitario = producto.Precio
                    });
                }

                // 4. Actualizar la interfaz
                ActualizarCarrito();
                LimpiarControlesAgregar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar producto: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Finaliza la venta y ejecuta la transacción
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (_vendedorActual == null || _vendedorActual.ID_Empleado <= 0)
            {
                MessageBox.Show("No hay un vendedor válido con ID asignado. Inicie sesión o revise la configuración de su usuario.", "Error de Vendedor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_carrito.Any())
            {
                MessageBox.Show("El carrito está vacío. Agregue productos antes de cobrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalFinal = _carrito.Sum(d => d.Subtotal);

            // Crear el objeto Venta (Encabezado)
            VentaPOJO nuevaVenta = new VentaPOJO
            {
                Total = totalFinal,
                ID_Empleado = _vendedorActual.ID_Empleado, // Usamos el ID del vendedor guardado
                Detalles = _carrito
            };

            try
            {
                // LLAMADA A LA CAPA DE DATOS CON TRANSACCIÓN
                bool exito = _ventaDAO.RegistrarVentaTransaccional(nuevaVenta);

                if (exito)
                {
                    MessageBox.Show($"Venta registrada con éxito. Total: {totalFinal:C2}", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    VaciarCarrito();
                }
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES: Muestra el mensaje de Rollback
                MessageBox.Show("Fallo la transacción de venta. " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarCarrito()
        {
            // Usar BindingSource o reasignar la fuente de datos para refrescar la DataGridView
            dataGridViewCarrito.DataSource = null;
            dataGridViewCarrito.DataSource = _carrito;

            // Calcular y mostrar el total
            decimal total = _carrito.Sum(d => d.Subtotal);
            lblTotal.Text = $"TOTAL: {total:C2}";
        }

        private void VaciarCarrito()
        {
            _carrito.Clear();
            ActualizarCarrito();
            LimpiarControlesAgregar();
        }

        private void LimpiarControlesAgregar()
        {
            txtISBN.Clear();
            numCantidad.Value = 1;
            txtISBN.Focus();
        }

    }
}
