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
        public bool InsertarEmpleado(EmpleadoPOJO empleado)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // Ejecutamos el Stored Procedure
                    MySqlCommand command = new MySqlCommand("sp_InsertarEmpleado", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Asignación de parámetros
                    command.Parameters.AddWithValue("p_Nombre", empleado.Nombre);
                    command.Parameters.AddWithValue("p_Apellido", empleado.Apellido);
                    command.Parameters.AddWithValue("p_Cargo", empleado.Cargo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al insertar empleado: " + ex.Message);
                return false;
            }
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
                    MySqlCommand command = new MySqlCommand("sp_ObtenerEmpleados", connection);
                    command.CommandType = CommandType.StoredProcedure; // Ejecutamos el Stored Procedure

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al obtener empleados: " + ex.Message);
            }
            return dt;
        }
    }
}
