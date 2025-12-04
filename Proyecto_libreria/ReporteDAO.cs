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
    /// Objeto de Acceso a Datos (DAO) para generar reportes complejos de ventas.
    /// </summary>
    public class ReporteDAO
    {
        private readonly string connectionString = Conexion.CadenaConexionMySQL;
        /// <summary>
        /// Genera el Reporte 1: Ventas por Vendedor en un Período de Tiempo.
        /// Ordena de mayor a menor por Monto Vendido.
        /// </summary>
        public DataTable ObtenerVentasPorVendedor(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Consulta: Suma el Total de ventas agrupado por Empleado en el rango de fechas.
                    string query = @"
                        SELECT 
                            E.ID_Empleado AS Clave,
                            CONCAT(E.Nombre, ' ', E.Apellido) AS NOMBRE,
                            SUM(V.Total) AS MONTO_VENDIDO
                        FROM VENTAS V
                        INNER JOIN EMPLEADOS E ON V.ID_Empleado = E.ID_Empleado
                        WHERE V.Fecha_Hora BETWEEN @fechaInicio AND @fechaFin
                        GROUP BY E.ID_Empleado, E.Nombre, E.Apellido
                        ORDER BY MONTO_VENDIDO DESC"; // Ordenar de mayor a menor (REQUISITO)

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Aseguramos que la fecha de fin incluya el final del día
                    command.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", fechaFin.Date.AddDays(1).AddSeconds(-1));

                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al generar Reporte de Ventas por Vendedor: " + ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// Genera el Reporte 2: Unidades Vendidas de Libros en un Mes y Año específicos.
        /// </summary>
        public DataTable ObtenerUnidadesVendidasPorLibro(int mes, int anio)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Consulta: Agrega la cantidad de libros vendidos por mes/año
                    string query = @"
                        SELECT
                            P.CLAVE AS ISBN,
                            P.NOMBRE AS TITULO,
                            P.DESCRIPCION AS DESCRIPCION,
                            P.PRECIO AS COSTO, -- Precio del catálogo (se puede modificar para usar Precio_Unitario de DETALLE_VENTA)
                            SUM(DV.Cantidad) AS UNIDADES_VENDIDAS
                        FROM DETALLE_VENTA DV
                        INNER JOIN VENTAS V ON DV.ID_Venta = V.ID_Venta
                        INNER JOIN PRODUCTOS P ON DV.CLAVE_Producto = P.CLAVE
                        WHERE MONTH(V.Fecha_Hora) = @mes AND YEAR(V.Fecha_Hora) = @anio
                        GROUP BY P.CLAVE, P.NOMBRE, P.DESCRIPCION, P.PRECIO
                        ORDER BY P.NOMBRE ASC"; // Ordenar alfabéticamente por nombre del libro (REQUISITO)

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@mes", mes);
                    command.Parameters.AddWithValue("@anio", anio);

                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al generar Reporte de Unidades Vendidas: " + ex.Message);
            }
            return dt;
        }
    }
}
