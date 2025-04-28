using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductoService : IService<Producto>, IProductoService
    {
        private readonly ProductoRepository repoProducto;

        public ProductoService()
        {
            repoProducto = new ProductoRepository(Archivos.ARC_PRODUCTO);
        }

        public string Guardar(Producto entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Error... el producto no puede ser nulo");
                }

                if (string.IsNullOrEmpty(entity.Referencia))
                {
                    throw new ArgumentException("Error... la referencia no puede estar vacía");
                }

                if (string.IsNullOrEmpty(entity.Nombre))
                {
                    throw new ArgumentException("Error... el nombre no puede estar vacío");
                }

                if (entity.Existencias < 0)
                {
                    throw new ArgumentException("Error... las existencias no pueden ser negativas");
                }

                if (entity.StockMinimo < 0)
                {
                    throw new ArgumentException("Error... el stock mínimo no puede ser negativo");
                }

                if (entity.PrecioUnitario <= 0)
                {
                    throw new ArgumentException("Error... el precio unitario debe ser mayor a cero");
                }

                if (string.IsNullOrEmpty(entity.Estado))
                {
                    throw new ArgumentException("Error... el estado no puede estar vacío");
                }

                if (entity.Estado.ToLower() != "activo" && entity.Estado.ToLower() != "inactivo")
                {
                    throw new ArgumentException("Error... el estado solo puede ser 'activo' o 'inactivo'");
                }

                if (repoProducto.ExisteReferencia(entity.Referencia))
                {
                    throw new ArgumentException("Error... ya existe un producto con esa referencia");
                }

                return repoProducto.Guardar(entity);
            }
            catch (Exception ex)
            {
                return $"Error al guardar producto: {ex.Message}";
            }
        }

        public List<Producto> Consultar()
        {
            return repoProducto.Consultar();
        }

        public string Modificar(Producto entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new NullReferenceException("Error... el producto no puede ser nulo");
                }

                if (string.IsNullOrEmpty(entity.Referencia))
                {
                    throw new ArgumentException("Error... la referencia no puede estar vacía");
                }

                if (string.IsNullOrEmpty(entity.Nombre))
                {
                    throw new ArgumentException("Error... el nombre no puede estar vacío");
                }

                if (entity.Existencias < 0)
                {
                    throw new ArgumentException("Error... las existencias no pueden ser negativas");
                }

                if (entity.StockMinimo < 0)
                {
                    throw new ArgumentException("Error... el stock mínimo no puede ser negativo");
                }

                if (entity.PrecioUnitario <= 0)
                {
                    throw new ArgumentException("Error... el precio unitario debe ser mayor a cero");
                }

                if (string.IsNullOrEmpty(entity.Estado))
                {
                    throw new ArgumentException("Error... el estado no puede estar vacío");
                }

                if (entity.Estado.ToLower() != "activo" && entity.Estado.ToLower() != "inactivo")
                {
                    throw new ArgumentException("Error... el estado solo puede ser 'activo' o 'inactivo'");
                }

                var productoExistente = repoProducto.ConsultarPorReferencia(entity.Referencia);
                if (productoExistente != null && productoExistente.Id != entity.Id)
                {
                    throw new ArgumentException("Error... ya existe otro producto con esa referencia");
                }

                return repoProducto.Modificar(entity);
            }
            catch (Exception ex)
            {
                return $"Error al modificar producto: {ex.Message}";
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

                return repoProducto.Eliminar(id);
            }
            catch (Exception ex)
            {
                return $"Error al eliminar producto: {ex.Message}";
            }
        }

        public Producto BuscarId(int id)
        {
            return Consultar().FirstOrDefault(p => p.Id == id);
        }

        public Producto BuscarPorReferencia(string referencia)
        {
            return repoProducto.ConsultarPorReferencia(referencia);
        }

        public List<Producto> ConsultarPorEstado(string estado)
        {
            return repoProducto.ConsultarPorEstado(estado);
        }

        public List<Producto> ConsultarPorNombre(string nombre)
        {
            return repoProducto.ConsultarPorNombre(nombre);
        }

        public string ActualizarExistencias(string referencia, int cantidad)
        {
            try
            {
                var producto = BuscarPorReferencia(referencia);
                if (producto == null)
                {
                    throw new ArgumentException("Error... no se encontró el producto con esa referencia");
                }

                if (producto.Existencias < cantidad)
                {
                    throw new ArgumentException($"Error... no hay suficientes existencias. Disponibles: {producto.Existencias}");
                }

                producto.Existencias -= cantidad;
                return Modificar(producto);
            }
            catch (Exception ex)
            {
                return $"Error al actualizar existencias: {ex.Message}";
            }
        }
    }
}
