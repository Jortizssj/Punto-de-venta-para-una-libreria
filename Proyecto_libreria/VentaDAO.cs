using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    internal class VentaDAO
    {
        private readonly string connectionString = Conexion.CadenaConexionMySQL;

        /// <summary>
        /// Registra una venta completa (VENTAS, DETALLE_VENTA y actualización de STOCK) como una transacción.
        /// </summary>
        /// <param name="venta">Objeto Venta con los detalles del carrito (productos).</param>
        /// <returns>True si la transacción fue exitosa, False si hubo rollback.</returns>
        public bool RegistrarVentaTransaccional(VentaPOJO venta)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                // 1. Iniciar la transacción
                transaction = connection.BeginTransaction();

                // 2. Insertar en la tabla VENTAS para obtener el ID_Venta
                string queryVenta = "INSERT INTO VENTAS (Total, ID_Empleado) VALUES (@Total, @ID_Empleado); SELECT LAST_INSERT_ID();";
                MySqlCommand cmdVenta = new MySqlCommand(queryVenta, connection, transaction);
                cmdVenta.Parameters.AddWithValue("@Total", venta.Total);
                cmdVenta.Parameters.AddWithValue("@ID_Empleado", venta.ID_Empleado);

                // Ejecutar y obtener el ID_Venta generado
                long idVentaGenerado = (long)cmdVenta.ExecuteScalar();

                // 3. Iterar sobre los detalles de la venta
                foreach (var detalle in venta.Detalles)
                {
                    // A. Insertar en DETALLE_VENTA
                    string queryDetalle = @"
                        INSERT INTO DETALLE_VENTA (ID_Venta, CLAVE_Producto, Cantidad, Precio_Unitario) 
                        VALUES (@ID_Venta, @Clave, @Cantidad, @Precio)";

                    MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, connection, transaction);
                    cmdDetalle.Parameters.AddWithValue("@ID_Venta", idVentaGenerado);
                    cmdDetalle.Parameters.AddWithValue("@Clave", detalle.ClaveProducto);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@Precio", detalle.PrecioUnitario);

                    cmdDetalle.ExecuteNonQuery();

                    // B. Actualizar/Disminuir STOCK en PRODUCTOS (Esta acción activa el TRIGGER de auditoría)
                    string queryStock = "UPDATE PRODUCTOS SET STOCK = STOCK - @Cantidad WHERE CLAVE = @Clave";

                    MySqlCommand cmdStock = new MySqlCommand(queryStock, connection, transaction);
                    cmdStock.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmdStock.Parameters.AddWithValue("@Clave", detalle.ClaveProducto);

                    int filasAfectadas = cmdStock.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                    {
                        // Si no se actualizó el stock (ej: no existe el producto o el stock es insuficiente), forzamos un rollback
                        throw new Exception($"No se pudo actualizar el stock para el producto: {detalle.ClaveProducto}.");
                    }
                }

                // 4. Si todo fue exitoso, confirmar la transacción
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // CONTROL DE ERRORES: Si algo falla, hacer rollback
                Console.WriteLine("Error en la transacción de venta: " + ex.Message);
                try
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transacción deshecha (ROLLBACK).");
                    }
                }
                catch (Exception rbEx)
                {
                    Console.WriteLine("Error al intentar hacer rollback: " + rbEx.Message);
                }
                // Relanzar la excepción para que la Capa de Presentación la maneje.
                throw new Exception("La venta no pudo ser completada. " + ex.Message, ex);
            }
            finally
            {
                // Cerrar la conexión
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
