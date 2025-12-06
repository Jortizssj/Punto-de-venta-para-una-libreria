using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_libreria
{
    /// <summary>
    /// Representa la entidad Producto, utilizada para las ventas.
    /// </summary>
    public class ProductoPOJO
    {
        public string ClaveISBN { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public ProductoPOJO()
        {
        }
    }
}
