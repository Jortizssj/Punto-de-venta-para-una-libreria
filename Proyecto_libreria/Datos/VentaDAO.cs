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

                transaction = connection.BeginTransaction();

                // 2. Insertar en la tabla VENTAS para obtener el ID_Venta
                string queryVenta = "INSERT INTO VENTAS (Total, ID_Empleado) VALUES (@Total, @ID_Empleado); SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);";
                MySqlCommand cmdVenta = new MySqlCommand(queryVenta, connection, transaction);
                cmdVenta.Parameters.AddWithValue("@Total", venta.Total);
                cmdVenta.Parameters.AddWithValue("@ID_Empleado", venta.ID_Empleado);

                object result = cmdVenta.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    throw new Exception("Error al obtener el ID de la venta recién insertada.");
                }

                long idVentaGenerado = Convert.ToInt64(result);

                foreach (var detalle in venta.Detalles)
                {
                    string queryDetalle = @"
                        INSERT INTO DETALLE_VENTA (ID_Venta, CLAVE_Producto, Cantidad, Precio_Unitario) 
                        VALUES (@ID_Venta, @Clave, @Cantidad, @Precio)";

                    MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, connection, transaction);
                    cmdDetalle.Parameters.AddWithValue("@ID_Venta", idVentaGenerado);
                    cmdDetalle.Parameters.AddWithValue("@Clave", detalle.ClaveProducto);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@Precio", detalle.PrecioUnitario);

                    cmdDetalle.ExecuteNonQuery();

                    // B. Actualizar STOCK en PRODUCTOS, activa el trigger
                    string queryStock = "UPDATE PRODUCTOS SET STOCK = STOCK - @Cantidad WHERE CLAVE = @Clave AND STOCK >= @Cantidad";

                    MySqlCommand cmdStock = new MySqlCommand(queryStock, connection, transaction);
                    cmdStock.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmdStock.Parameters.AddWithValue("@Clave", detalle.ClaveProducto);

                    int filasAfectadas = cmdStock.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                    {
                        throw new Exception($"No se pudo actualizar el stock para el producto: {detalle.ClaveProducto}.");
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en la transacción de venta: " + ex.Message);
                try
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transacción deshecha.");
                    }
                }
                catch (Exception rbEx)
                {
                    Console.WriteLine("Error al intentar hacer rollback: " + rbEx.Message);
                }
                throw new Exception("La venta no pudo ser completada. " + ex.Message, ex);
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
