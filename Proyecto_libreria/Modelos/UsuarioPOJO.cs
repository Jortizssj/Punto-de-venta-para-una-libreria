using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Representa la entidad Usuario, utilizada para el control de acceso.
    /// </summary>
    public class UsuarioPOJO
    {
        public int ID_Usuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Tipo_Usuario { get; set; } // Administrador o Vendedor
        public int ID_Empleado { get; set; }
    }
}
