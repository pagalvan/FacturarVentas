using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class DetalleFactura : BaseEntity
    {
        public int IdFactura { get; set; }
        public string ReferenciaProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ValorItemVendido { get; set; }

        public DetalleFactura()
        {
        }

        public DetalleFactura(int idFactura, string referenciaProducto, string nombreProducto, int cantidad, decimal precioUnitario)
        {
            IdFactura = idFactura;
            ReferenciaProducto = referenciaProducto;
            NombreProducto = nombreProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            CalcularValorItemVendido();
        }

        public void CalcularValorItemVendido()
        {
            ValorItemVendido = PrecioUnitario * Cantidad;
        }

        public override string ToString()
        {
            return $"{Id};{IdFactura};{ReferenciaProducto};{NombreProducto};{Cantidad};{PrecioUnitario};{ValorItemVendido}";
        }
    }
}
