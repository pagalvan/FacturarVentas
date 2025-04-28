using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IProductoService : IService<Producto>
    {
        List<Producto> ConsultarPorNombre(string nombre);
        List<Producto> ConsultarPorEstado(string estado);
        Producto BuscarPorReferencia(string referencia);
    }
}
