using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    internal class ProductoDAO
    {
        // TODO: Reemplazar con tu cadena de conexión real
        private string connectionString = "Data Source=.;Initial Catalog=VENTAS;Integrated Security=True";

        // Método para obtener todos los productos
        public DataTable ObtenerTodosProductos()
        {
            DataTable dt = new DataTable();
            // Uso de TRY-CATCH-FINALLY para control de errores
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Consulta simple para mostrar en la cuadrícula
                    string query = "SELECT CLAVE as ISBN, NOMBRE, DESCRIPCION, PRECIO, STOCK FROM PRODUCTOS ORDER BY NOMBRE";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error de BD al obtener productos: " + ex.Message);
            }
            finally
            {
            }

            return dt;
        }

        // Método para registrar un nuevo producto (INSERT)
        public bool AgregarProducto(clsProducto producto)
        {
            bool exito = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO PRODUCTOS (CLAVE, NOMBRE, DESCRIPCION, PRECIO, STOCK) 
                        VALUES (@Clave, @Nombre, @Descripcion, @Precio, @Stock)";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Asignación de parámetros
                    command.Parameters.AddWithValue("@Clave", producto.ClaveISBN);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);

                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        exito = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error de BD al agregar producto: " + ex.Message);
                exito = false;
            }
            return exito;
        }

    }
}
}
