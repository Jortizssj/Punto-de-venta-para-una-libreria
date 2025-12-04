using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Objeto de Acceso a Datos (DAO) para la entidad Usuario.
    /// Maneja la lógica de autenticación (Login).
    /// </summary>
    public class UsuarioDAO
    {
        /// <summary>
        /// Verifica las credenciales del usuario contra la base de datos.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <param name="passwordTextoPlano">Contraseña ingresada por el usuario.</param>
        /// <returns>Objeto UsuarioPOJO si las credenciales son válidas, de lo contrario, null.</returns>
        public static UsuarioPOJO ValidarCredenciales(string nombreUsuario, string passwordTextoPlano)
        {
            UsuarioPOJO usuario = null;

            // 1. Obtener el hash de la contraseña ingresada
            string hashIngresado = Seguridad.ObtenerHashSHA256(passwordTextoPlano);

            // Uso de TRY-CATCH-FINALLY para control de errores (REQUISITO DE COTEJO)
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Conexion.CadenaConexionMySQL))
                {
                    connection.Open();

                    // La consulta busca un usuario donde coincidan el NombreUsuario y el Hash.
                    string query = @"
                    SELECT ID_Usuario, NombreUsuario, Tipo_Usuario, ID_Empleado
                    FROM USUARIOS
                    WHERE NombreUsuario = @user AND Contrasena_Hash = @hash";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user", nombreUsuario);
                        command.Parameters.AddWithValue("@hash", hashIngresado);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener ordinals una vez
                                int ordId = reader.GetOrdinal("ID_Usuario");
                                int ordNombre = reader.GetOrdinal("NombreUsuario");
                                int ordTipo = reader.GetOrdinal("Tipo_Usuario");
                                int ordEmpleado = reader.GetOrdinal("ID_Empleado");

                                usuario = new UsuarioPOJO
                                {
                                    ID_Usuario = reader.GetInt32(ordId),
                                    NombreUsuario = reader.IsDBNull(ordNombre) ? string.Empty : reader.GetString(ordNombre),
                                    Tipo_Usuario = reader.IsDBNull(ordTipo) ? string.Empty : reader.GetString(ordTipo),

                                    // Verifica si es NULL. Si es NULL, asigna 0. Si no, lee el entero.
                                    ID_Empleado = reader.IsDBNull(ordEmpleado) ? 0 : reader.GetInt32(ordEmpleado)
                                };
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Control de errores de la base de datos
                Console.WriteLine("Error de BD durante el login: " + ex.Message);
                throw new Exception("Error al conectar con la base de datos. Consulte al administrador.", ex);
            }
            return usuario;
        }
        public bool RegistrarUsuario(UsuarioPOJO usuario, string passwordPlano)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Conexion.CadenaConexionMySQL))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO USUARIOS (NombreUsuario, Contrasena_Hash, Tipo_Usuario, ID_Empleado) 
                        VALUES (@User, @Hash, @Tipo, @IdEmp)";

                    // Encriptar la contraseña nueva antes de guardarla
                    string hash = Seguridad.ObtenerHashSHA256(passwordPlano);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@User", usuario.NombreUsuario);
                        command.Parameters.AddWithValue("@Hash", hash);
                        command.Parameters.AddWithValue("@Tipo", usuario.Tipo_Usuario);
                        command.Parameters.AddWithValue("@IdEmp", usuario.ID_Empleado);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al registrar usuario: " + ex.Message);
                return false;
            }
        }
        public bool ActualizarTipoUsuario(int idEmpleado, string nuevoTipo)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Conexion.CadenaConexionMySQL))
                {
                    connection.Open();
                    string query = "UPDATE USUARIOS SET Tipo_Usuario = @Tipo WHERE ID_Empleado = @IdEmp";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Tipo", nuevoTipo);
                        command.Parameters.AddWithValue("@IdEmp", idEmpleado);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de BD al actualizar rol: " + ex.Message);
                return false;
            }
        }
    }
}
