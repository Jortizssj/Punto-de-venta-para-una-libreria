using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Clase para manejar la cadena de conexión a la base de datos MySQL.
    /// </summary>
    public static class Conexion
    {
        public static string CadenaConexionMySQL =
            "Server=localhost;Database=VENTAS;Uid=root;Pwd=root;";
    }
}
