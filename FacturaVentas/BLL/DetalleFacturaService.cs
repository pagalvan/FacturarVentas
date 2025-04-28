using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DetalleFacturaService : IService<DetalleFactura>
    {
        private readonly DetalleFacturaRepository repoDetalleFactura;

        public DetalleFacturaService()
        {
            repoDetalleFactura = new DetalleFacturaRepository(Archivos.ARC_DETALLE_FACTURA);
        }

        public string Guardar(DetalleFactura entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Error... el detalle de factura no puede ser nulo");
                }

                if (entity.IdFactura <= 0)
                {
                    throw new ArgumentException("Error... el id de factura debe ser mayor a cero");
                }

                if (string.IsNullOrEmpty(entity.ReferenciaProducto))
                {
                    throw new ArgumentException("Error... la referencia del producto no puede estar vacía");
                }

                if (string.IsNullOrEmpty(entity.NombreProducto))
                {
                    throw new ArgumentException("Error... el nombre del producto no puede estar vacío");
                }

                if (entity.Cantidad <= 0)
                {
                    throw new ArgumentException("Error... la cantidad debe ser mayor a cero");
                }

                if (entity.PrecioUnitario <= 0)
                {
                    throw new ArgumentException("Error... el precio unitario debe ser mayor a cero");
                }

                return repoDetalleFactura.Guardar(entity);
            }
            catch (Exception ex)
            {
                return $"Error al guardar detalle de factura: {ex.Message}";
            }
        }

        public List<DetalleFactura> Consultar()
        {
            return repoDetalleFactura.Consultar();
        }

        public string Modificar(DetalleFactura entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Error... el detalle de factura no puede ser nulo");
                }

                return repoDetalleFactura.Modificar(entity);
            }
            catch (Exception ex)
            {
                return $"Error al modificar detalle de factura: {ex.Message}";
            }
        }

        public string Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Error... el id debe ser mayor a cero");
                }

                return repoDetalleFactura.Eliminar(id);
            }
            catch (Exception ex)
            {
                return $"Error al eliminar detalle de factura: {ex.Message}";
            }
        }

        public DetalleFactura BuscarId(int id)
        {
            return Consultar().FirstOrDefault(d => d.Id == id);
        }

        public List<DetalleFactura> ConsultarPorFactura(int idFactura)
        {
            return repoDetalleFactura.ConsultarPorFactura(idFactura);
        }
    }
}
