using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public abstract class FileRepository<T> : IRepository<T>
    {
        protected string ruta;

        public FileRepository(string ruta)
        {
            this.ruta = ruta;
        }

        public virtual string Guardar(T entity)
        {
            try
            {
                // Asignar un ID único si es 0 (nuevo registro)
                if (GetId(entity) == 0)
                {
                    AsignarIdUnico(entity);
                }

                StreamWriter sw = new StreamWriter(ruta, true);
                sw.WriteLine(entity.ToString());
                sw.Close();
                return "Guardado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error al guardar: {ex.Message}";
            }
        }

        protected virtual void AsignarIdUnico(T entity)
        {
            // Obtener el ID más alto actual
            int maxId = 0;
            List<T> lista = Consultar();

            foreach (var item in lista)
            {
                int id = GetId(item);
                if (id > maxId)
                {
                    maxId = id;
                }
            }

            // Asignar el siguiente ID
            int nuevoId = maxId + 1;

            // Establecer el nuevo ID en la entidad
            var property = entity.GetType().GetProperty("Id");
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, nuevoId);
            }
        }

        public abstract List<T> Consultar();
        public abstract T Mappear(string datos);

        public virtual string Modificar(T entity)
        {
            try
            {
                List<T> lista = Consultar();
                int entityId = GetId(entity);

                if (entityId == 0)
                {
                    return "Error: No se puede modificar una entidad sin ID válido";
                }

                // Crear una lista temporal con los elementos actualizados
                List<T> listaActualizada = new List<T>();
                bool encontrado = false;

                foreach (var item in lista)
                {
                    int itemId = GetId(item);
                    if (itemId == entityId)
                    {
                        listaActualizada.Add(entity); // Añadir el elemento modificado
                        encontrado = true;
                    }
                    else
                    {
                        listaActualizada.Add(item); // Mantener los elementos no modificados
                    }
                }

                if (!encontrado)
                {
                    return "Error: No se encontró la entidad a modificar";
                }

                // Reescribir todo el archivo
                File.Delete(ruta);
                foreach (var item in listaActualizada)
                {
                    // Usar un método que no asigne nuevos IDs
                    GuardarSinAsignarId(item);
                }

                return "Modificado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error al modificar: {ex.Message}";
            }
        }

        // Método auxiliar para guardar sin asignar nuevo ID
        protected virtual void GuardarSinAsignarId(T entity)
        {
            try
            {
                StreamWriter sw = new StreamWriter(ruta, true);
                sw.WriteLine(entity.ToString());
                sw.Close();
            }
            catch (Exception)
            {
                // Manejar excepción si es necesario
            }
        }

        public virtual string Eliminar(int id)
        {
            try
            {
                List<T> lista = Consultar();
                File.Delete(ruta);
                foreach (var item in lista)
                {
                    if (!GetId(item).Equals(id))
                    {
                        Guardar(item);
                    }
                }
                return "Eliminado correctamente";
            }
            catch (Exception ex)
            {
                return $"Error al eliminar: {ex.Message}";
            }
        }

        protected virtual int GetId(T entity)
        {
            var property = entity.GetType().GetProperty("Id");
            if (property != null)
            {
                return (int)property.GetValue(entity);
            }
            return 0;
        }
    }
}
