using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    internal class ProductoDAO
    {
        private readonly string connectionString = Conexion.CadenaConexionMySQL;

        public DataTable ObtenerTodosProductos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand("SELECT CLAVE as ISBN, NOMBRE, DESCRIPCION, PRECIO, STOCK FROM PRODUCTOS ORDER BY NOMBRE", connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    connection.Open();
                    adapter.Fill(dt);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al obtener productos: " + ex.Message);
            }
            return dt;
        }

        // Método para registrar un nuevo producto (INSERT)
        public bool AgregarProducto(ProductoPOJO producto)
        {
            bool exito = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(@"
                        INSERT INTO PRODUCTOS (CLAVE, NOMBRE, DESCRIPCION, PRECIO, STOCK) 
                        VALUES (@Clave, @Nombre, @Descripcion, @Precio, @Stock)", connection))
                {
                    command.Parameters.AddWithValue("@Clave", producto.ClaveISBN);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);

                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                    exito = filasAfectadas > 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al agregar producto: " + ex.Message);
                exito = false;
            }
            return exito;
        }

        public bool ActualizarProducto(ProductoPOJO producto)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(@"
                        UPDATE PRODUCTOS SET 
                        NOMBRE = @Nombre, 
                        DESCRIPCION = @Descripcion, 
                        PRECIO = @Precio, 
                        STOCK = @Stock 
                        WHERE CLAVE = @Clave", connection))
                {
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.Parameters.AddWithValue("@Clave", producto.ClaveISBN);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al actualizar producto: " + ex.Message);
                return false;
            }
        }

        public bool EliminarProducto(string claveISBN)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand("DELETE FROM PRODUCTOS WHERE CLAVE = @Clave", connection))
                {
                    command.Parameters.AddWithValue("@Clave", claveISBN);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al intentar eliminar producto: " + ex.Message);
                if (ex.Number == 1451)
                {
                    throw new Exception("No se puede eliminar el producto porque tiene ventas asociadas (clave foránea).");
                }
                return false;
            }
        }

        // Buscar producto por ISBN (clave)
        public ProductoPOJO BuscarProductoPorISBN(string isbn)
        {
            ProductoPOJO producto = null;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(@"
                    SELECT CLAVE, NOMBRE, DESCRIPCION, PRECIO, STOCK 
                    FROM PRODUCTOS 
                    WHERE CLAVE = @isbn", connection))
                {
                    string isbnLimpio = (isbn ?? string.Empty).Trim();
                    command.Parameters.AddWithValue("@isbn", isbnLimpio);

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int ordClave = reader.GetOrdinal("CLAVE");
                            int ordNombre = reader.GetOrdinal("NOMBRE");
                            int ordDesc = reader.GetOrdinal("DESCRIPCION");
                            int ordPrecio = reader.GetOrdinal("PRECIO");
                            int ordStock = reader.GetOrdinal("STOCK");

                            producto = new ProductoPOJO
                            {
                                ClaveISBN = reader.IsDBNull(ordClave) ? null : reader.GetString(ordClave),
                                Nombre = reader.IsDBNull(ordNombre) ? null : reader.GetString(ordNombre),
                                Descripcion = reader.IsDBNull(ordDesc) ? null : reader.GetString(ordDesc),
                                Precio = reader.IsDBNull(ordPrecio) ? 0m : reader.GetDecimal(ordPrecio),
                                Stock = reader.IsDBNull(ordStock) ? 0 : reader.GetInt32(ordStock)
                            };
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al buscar producto: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado al buscar producto: " + ex.Message);
            }
            return producto;
        }
    }
}
