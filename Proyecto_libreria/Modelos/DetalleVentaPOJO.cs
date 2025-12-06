using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Representa un ítem de la venta
    /// </summary>
    public class DetalleVentaPOJO
    {
        public int ID_Detalle { get; set; }
        public int ID_Venta { get; set; }
        public string ClaveProducto { get; set; } // ISBN
        public string NombreProducto { get; set; } // Nombre del producto (para UI/Reportes)
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Propiedad calculada para el Subtotal de la línea.
        /// </summary>
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

}
