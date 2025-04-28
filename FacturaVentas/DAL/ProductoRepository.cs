using ENTITY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ProductoRepository : FileRepository<Producto>
    {
        public ProductoRepository(string ruta) : base(ruta)
        {
        }

        public override List<Producto> Consultar()
        {
            try
            {
                List<Producto> lista = new List<Producto>();

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

        public override Producto Mappear(string datos)
        {
            string[] campos = datos.Split(';');
            Producto producto = new Producto();
            producto.Id = int.Parse(campos[0]);
            producto.Referencia = campos[1];
            producto.Nombre = campos[2];
            producto.Existencias = int.Parse(campos[3]);
            producto.StockMinimo = int.Parse(campos[4]);
            producto.PrecioUnitario = decimal.Parse(campos[5]);
            producto.Estado = campos[6];
            return producto;
        }

        public Producto ConsultarPorReferencia(string referencia)
        {
            return Consultar().FirstOrDefault(p => p.Referencia == referencia);
        }

        public List<Producto> ConsultarPorEstado(string estado)
        {
            return Consultar().Where(p => p.Estado.ToLower() == estado.ToLower()).ToList();
        }

        public List<Producto> ConsultarPorNombre(string nombre)
        {
            return Consultar().Where(p => p.Nombre.ToLower().Contains(nombre.ToLower())).ToList();
        }

        public bool ExisteReferencia(string referencia)
        {
            return Consultar().Any(p => p.Referencia == referencia);
        }
    }
}
