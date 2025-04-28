using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Factura : BaseEntity
    {
        public DateTime Fecha { get; set; }
        public decimal ValorTotal { get; set; }
        public List<DetalleFactura> Detalles { get; set; }

        public Factura()
        {
            Detalles = new List<DetalleFactura>();
        }

        public Factura(DateTime fecha)
        {
            Fecha = fecha;
            ValorTotal = 0;
            Detalles = new List<DetalleFactura>();
        }

        public void AgregarDetalle(DetalleFactura detalle)
        {
            if (detalle != null)
            {
                Detalles.Add(detalle);
                CalcularValorTotal();
            }
        }

        public void CalcularValorTotal()
        {
            ValorTotal = Detalles.Sum(d => d.ValorItemVendido);
        }

        public override string ToString()
        {
            return $"{Id};{Fecha:yyyy-MM-dd};{ValorTotal}";
        }
    }
}
