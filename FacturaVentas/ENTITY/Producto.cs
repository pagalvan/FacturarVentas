using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Producto : BaseEntity
    {
        public string Referencia { get; set; }
        public string Nombre { get; set; }
        public int Existencias { get; set; }
        public int StockMinimo { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Estado { get; set; }

        public Producto()
        {
        }

        public Producto(string referencia, string nombre, int existencias, int stockMinimo, decimal precioUnitario, string estado)
        {
            Referencia = referencia;
            Nombre = nombre;
            Existencias = existencias;
            StockMinimo = stockMinimo;
            PrecioUnitario = precioUnitario;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"{Id};{Referencia};{Nombre};{Existencias};{StockMinimo};{PrecioUnitario};{Estado}";
        }
    }
}
