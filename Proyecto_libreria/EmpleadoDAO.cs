using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    internal class EmpleadoDAO
    {
        private readonly string connectionString = Conexion.CadenaConexionMySQL;

        // Uso de TRY-CATCH-FINALLY para control de errores (REQUISITO DE COTEJO)

        /// <summary>
        /// Inserta un nuevo empleado utilizando el Stored Procedure 'sp_InsertarEmpleado'.
        /// </summary>
        /// <param name="empleado">Objeto empleado con los datos a insertar.</param>
        /// <returns>El ID generado (int). Retorna 0 en caso de error.</returns>
        public int InsertarEmpleado(EmpleadoPOJO empleado)
        {
            int idGenerado = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 1. Insertar el Empleado usando el Stored Procedure
                    MySqlCommand command = new MySqlCommand("sp_InsertarEmpleado", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("p_Nombre", empleado.Nombre);
                    command.Parameters.AddWithValue("p_Apellido", empleado.Apellido);
                    command.Parameters.AddWithValue("p_Cargo", empleado.Cargo);

                    command.ExecuteNonQuery();

                    // 2. Recuperar el ID generado en la misma conexión
                    // Usamos CAST para asegurar compatibilidad de tipos con el conector MySQL
                    MySqlCommand cmdId = new MySqlCommand("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED);", connection);

                    object result = cmdId.ExecuteScalar();
                    if (result != null)
                    {
                        idGenerado = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar empleado: " + ex.Message);
                return 0; // Retorna 0 en caso de error
            }
            return idGenerado; // Retorna el ID (int)
        }

        /// <summary>
        /// Actualiza un empleado existente utilizando el Stored Procedure 'sp_ActualizarEmpleado'.
        /// </summary>
        public bool ActualizarEmpleado(EmpleadoPOJO empleado)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("sp_ActualizarEmpleado", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Asignación de parámetros
                    command.Parameters.AddWithValue("p_ID_Empleado", empleado.ID_Empleado);
                    command.Parameters.AddWithValue("p_Nombre", empleado.Nombre);
                    command.Parameters.AddWithValue("p_Apellido", empleado.Apellido);
                    command.Parameters.AddWithValue("p_Cargo", empleado.Cargo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al actualizar empleado: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina lógicamente un empleado utilizando el Stored Procedure 'sp_EliminarEmpleado'.
        /// </summary>
        public bool EliminarEmpleado(int idEmpleado)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("sp_EliminarEmpleado", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("p_ID_Empleado", idEmpleado);

                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al eliminar empleado: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los empleados activos para mostrar en la cuadrícula.
        /// </summary>
        public DataTable ObtenerTodosEmpleados()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // Hacemos JOIN para traer el Usuario y el Tipo asociado al empleado
                    string query = @"
                        SELECT 
                            E.ID_Empleado, 
                            E.Nombre, 
                            E.Apellido, 
                            E.Cargo, 
                            E.Activo,
                            U.NombreUsuario,
                            U.Tipo_Usuario
                        FROM EMPLEADOS E
                        LEFT JOIN USUARIOS U ON E.ID_Empleado = U.ID_Empleado
                        WHERE E.Activo = 1
                        ORDER BY E.Apellido, E.Nombre";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener empleados: " + ex.Message);
            }
            return dt;
        }

    }
}
