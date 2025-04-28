using ENTITY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DetalleFacturaRepository : FileRepository<DetalleFactura>
    {
        public DetalleFacturaRepository(string ruta) : base(ruta)
        {
        }

        public override List<DetalleFactura> Consultar()
        {
            try
            {
                List<DetalleFactura> lista = new List<DetalleFactura>();

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

        public override DetalleFactura Mappear(string datos)
        {
            string[] campos = datos.Split(';');
            DetalleFactura detalle = new DetalleFactura();
            detalle.Id = int.Parse(campos[0]);
            detalle.IdFactura = int.Parse(campos[1]);
            detalle.ReferenciaProducto = campos[2];
            detalle.NombreProducto = campos[3];
            detalle.Cantidad = int.Parse(campos[4]);
            detalle.PrecioUnitario = decimal.Parse(campos[5]);
            detalle.ValorItemVendido = decimal.Parse(campos[6]);
            return detalle;
        }

        public List<DetalleFactura> ConsultarPorFactura(int idFactura)
        {
            return Consultar().Where(d => d.IdFactura == idFactura).ToList();
        }
    }
}
