using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Representa la entidad Empleado para el catálogo.
    /// </summary>
    public class EmpleadoPOJO
    {
        public int ID_Empleado { get; set; } 
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cargo { get; set; }
        public bool Activo { get; set; }
    }
}
