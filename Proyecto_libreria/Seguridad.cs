using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Clase estática para manejar las operaciones de seguridad,
    /// En este caso la encriptacion de contraseñas utilizando SHA-256.
    /// </summary>
    internal class Seguridad
    {
        /// <summary>
        /// Aplica un hash SHA-256 a la contraseña proporcionada.
        /// </summary>
        /// <param name="password">Contraseña en texto plano.</param>
        /// <returns>Hash SHA-256 en formato hexadecimal.</returns>
        public static string ObtenerHashSHA256(string password)
        {
            // Usamos SHA256 para ser consistente con la función SHA2(X, 256) de MySQL
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir el string a bytes para el hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir el array de bytes a un string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                // Nota importante: MySQL SHA2(X, 256) devuelve un string de 64 caracteres.
                // Esta implementación de C# también lo hace, asegurando la compatibilidad.
                return builder.ToString();
            }
        }
    }
}
