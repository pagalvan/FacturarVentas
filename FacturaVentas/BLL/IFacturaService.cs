using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IFacturaService : IService<Factura>
    {
        string ValidarDisponibilidadProducto(string referencia, int cantidad);
    }
}
