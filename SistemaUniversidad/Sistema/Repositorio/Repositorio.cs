using SistemaUniversidad.Personas.Interfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Personas.Repositorio
{
    // Clase Repositorio genérico
    public class Repositorio<T> where T : IIdentificable
    {
        private readonly Dictionary<string, T> items = new Dictionary<string, T>();


       //METODOS CRUD
        public void Agregar(T item)
        {
            if (items.ContainsKey(item.Identificacion))
                throw new InvalidOperationException($"Ya existe un elemento con ID: {item.Identificacion}");

            items.Add(item.Identificacion, item);
        }

        
        public bool Eliminar(string id)
        {
            
            return items.Remove(id);
        }

        
        public T BuscarPorId(string id)
        {
            
            if (items.TryGetValue(id, out T item))
            {
                return item;
            }
            
            return default(T);
        }

        public List<T> ObtenerTodos()
        {
          
            return items.Values.ToList();
        }

        
        public List<T> Buscar(Func<T, bool> predicado)
        {
            
            return items.Values.Where(predicado).ToList();
        }


    }
}