using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Objeto de Acceso a Datos (DAO) para el Monitoreo/Auditoría de Productos.
    /// </summary>
    public class AuditoriaDAO
    {
        private readonly string connectionString = Conexion.CadenaConexionMySQL;

        /// <summary>
        /// Obtiene todos los registros de auditoría de productos, 
        /// ordenados del más reciente al más antiguo.
        /// </summary>
        public DataTable ObtenerHistorialAuditoria()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // La consulta ordena por Fecha_Hora descendente (más reciente primero)
                    string query = @"
                        SELECT 
                            Fecha_Hora, 
                            Tipo_Cambio, 
                            Usuario_DB, 
                            CLAVE_Producto_Afectado AS ISBN,
                            Campo_Afectado, 
                            Valor_Anterior, 
                            Valor_Nuevo
                        FROM AUDITORIA_PRODUCTOS 
                        ORDER BY Fecha_Hora DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al obtener auditoría: " + ex.Message);
            }
            return dt;
        }
    }
}
