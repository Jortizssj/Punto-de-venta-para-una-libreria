using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    public class VentaPOJO
    {
        public int ID_Venta { get; set; }
        public DateTime Fecha_Hora { get; set; }
        public decimal Total { get; set; }
        public int ID_Empleado { get; set; }
        public List<DetalleVentaPOJO> Detalles { get; set; }

        public VentaPOJO()
        {
            // Inicializar la lista para evitar errores de referencia nula (NullReferenceException)
            Detalles = new List<DetalleVentaPOJO>();
        }
    }
}
