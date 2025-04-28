using ENTITY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class FacturaRepository : FileRepository<Factura>
    {
        private DetalleFacturaRepository detalleFacturaRepository;

        public FacturaRepository(string ruta) : base(ruta)
        {
            detalleFacturaRepository = new DetalleFacturaRepository(Archivos.ARC_DETALLE_FACTURA);
        }

        public override List<Factura> Consultar()
        {
            try
            {
                List<Factura> lista = new List<Factura>();

                if (File.Exists(ruta))
                {
                    StreamReader sr = new StreamReader(ruta);
                    while (!sr.EndOfStream)
                    {
                        lista.Add(Mappear(sr.ReadLine()));
                    }
                    sr.Close();
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Factura Mappear(string datos)
        {
            string[] campos = datos.Split(';');
            Factura factura = new Factura();
            factura.Id = int.Parse(campos[0]);
            factura.Fecha = DateTime.Parse(campos[1]);
            factura.ValorTotal = decimal.Parse(campos[2]);

            // Cargar detalles de factura
            List<DetalleFactura> detalles = detalleFacturaRepository.ConsultarPorFactura(factura.Id);
            foreach (var detalle in detalles)
            {
                factura.AgregarDetalle(detalle);
            }

            return factura;
        }

        public DateTime ObtenerFechaUltimaFactura()
        {
            var facturas = Consultar();
            if (facturas.Count > 0)
            {
                return facturas.Max(f => f.Fecha);
            }
            return DateTime.MinValue;
        }
    }
}
