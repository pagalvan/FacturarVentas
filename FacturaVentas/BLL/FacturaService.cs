using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class FacturaService : IService<Factura>, IFacturaService
    {
        private readonly FacturaRepository repoFactura;
        private readonly ProductoService productoService;

        public FacturaService()
        {
            repoFactura = new FacturaRepository(Archivos.ARC_FACTURA);
            productoService = new ProductoService();
        }

        public string Guardar(Factura entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Error... la factura no puede ser nula");
                }

                // Validar que la fecha de la factura sea mayor o igual a la última factura
                DateTime fechaUltimaFactura = repoFactura.ObtenerFechaUltimaFactura();
                if (fechaUltimaFactura != DateTime.MinValue && entity.Fecha < fechaUltimaFactura)
                {
                    throw new ArgumentException($"Error... la fecha de la factura debe ser mayor o igual a {fechaUltimaFactura.ToShortDateString()}");
                }

                if (entity.Detalles == null || entity.Detalles.Count == 0)
                {
                    throw new ArgumentException("Error... la factura debe tener al menos un detalle");
                }

                // Guardar la factura
                string resultadoFactura = repoFactura.Guardar(entity);
                if (!resultadoFactura.StartsWith("Guardado"))
                {
                    return resultadoFactura;
                }

                // Actualizar existencias de productos
                foreach (var detalle in entity.Detalles)
                {
                    detalle.IdFactura = entity.Id;
                    string resultadoExistencias = productoService.ActualizarExistencias(detalle.ReferenciaProducto, detalle.Cantidad);
                    if (!resultadoExistencias.StartsWith("Modificado"))
                    {
                        return resultadoExistencias;
                    }
                }

                // Guardar detalles de factura
                DetalleFacturaService detalleService = new DetalleFacturaService();
                foreach (var detalle in entity.Detalles)
                {
                    string resultadoDetalle = detalleService.Guardar(detalle);
                    if (!resultadoDetalle.StartsWith("Guardado"))
                    {
                        return resultadoDetalle;
                    }
                }

                return "Factura guardada correctamente";
            }
            catch (Exception ex)
            {
                return $"Error al guardar factura: {ex.Message}";
            }
        }

        public List<Factura> Consultar()
        {
            return repoFactura.Consultar();
        }

        public string Modificar(Factura entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Error... la factura no puede ser nula");
                }

                return repoFactura.Modificar(entity);
            }
            catch (Exception ex)
            {
                return $"Error al modificar factura: {ex.Message}";
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

                return repoFactura.Eliminar(id);
            }
            catch (Exception ex)
            {
                return $"Error al eliminar factura: {ex.Message}";
            }
        }

        public Factura BuscarId(int id)
        {
            return Consultar().FirstOrDefault(f => f.Id == id);
        }

        public string ValidarDisponibilidadProducto(string referencia, int cantidad)
        {
            try
            {
                var producto = productoService.BuscarPorReferencia(referencia);
                if (producto == null)
                {
                    return $"Error: No se encontró el producto con referencia {referencia}";
                }

                if (producto.Existencias < cantidad)
                {
                    return $"No hay suficientes existencias. Disponibles: {producto.Existencias}";
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return $"Error al validar disponibilidad: {ex.Message}";
            }
        }

        public DetalleFactura CrearDetalleFactura(string referencia, int cantidad)
        {
            var producto = productoService.BuscarPorReferencia(referencia);
            if (producto == null)
            {
                throw new ArgumentException($"No se encontró el producto con referencia {referencia}");
            }

            if (producto.Existencias < cantidad)
            {
                throw new ArgumentException($"No hay suficientes existencias. Disponibles: {producto.Existencias}");
            }

            DetalleFactura detalle = new DetalleFactura();
            detalle.ReferenciaProducto = producto.Referencia;
            detalle.NombreProducto = producto.Nombre;
            detalle.Cantidad = cantidad;
            detalle.PrecioUnitario = producto.PrecioUnitario;
            detalle.CalcularValorItemVendido();

            return detalle;
        }
    }
}
