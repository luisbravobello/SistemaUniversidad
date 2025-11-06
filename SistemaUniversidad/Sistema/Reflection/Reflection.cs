using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Sistema.Reflection
{
    // Clase AnalizadorReflection

    public static class AnalizadorReflection
    {
        public static void MostrarPropiedades(Type tipo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n** PROPIEDADES PÚBLICAS de {tipo.Name} **");
            Console.ResetColor();

            
            PropertyInfo[] propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in propiedades)
            {
                Console.WriteLine($"- {prop.Name,-20} | Tipo: {prop.PropertyType.Name}");
            }
        }

        // METODOS
        public static void MostrarMetodos(Type tipo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n** MÉTODOS PÚBLICOS de {tipo.Name} **");
            Console.ResetColor();

            // Obtiene todos los métodos públicos (excluyendo métodos de propiedades y métodos base de Object)
            MethodInfo[] metodos = tipo.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object))
                                       .ToArray();

            foreach (var metodo in metodos.DistinctBy(m => m.Name))
            {
                var parametros = metodo.GetParameters()
                                       .Select(p => $"{p.ParameterType.Name} {p.Name}");

                Console.WriteLine($"- {metodo.ReturnType.Name,-8} {metodo.Name}({string.Join(", ", parametros)})");
            }
        }

        // Instancia Dinamica
        public static object CrearInstanciaDinamica(Type tipo, params object[] parametros)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nIntentando crear instancia dinámica de {tipo.Name}...");
            object nuevaInstancia = null;

            try
            {
                nuevaInstancia = Activator.CreateInstance(tipo, parametros);
                return nuevaInstancia;
            }
            catch (TargetInvocationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR DE CONSTRUCTOR (Validación fallida): {ex.InnerException?.Message ?? ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR DE REFLECTION: {ex.Message}");
                return null;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        // METODO INVOCAR
        public static object InvocarMetodo(object instancia, string nombreMetodo, params object[] parametros)
        {
          

            if (instancia == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: La instancia es nula. No se puede invocar '{nombreMetodo}'.");
                Console.ResetColor();
                return null;
            }

            Type tipo = instancia.GetType();

            try
            {
                MethodInfo metodo = tipo.GetMethod(nombreMetodo);
                if (metodo == null)
                {
                    throw new MissingMethodException($"Método '{nombreMetodo}' no encontrado en la clase {tipo.Name}.");
                }

                return metodo.Invoke(instancia, parametros);
            }
            catch (TargetInvocationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR DURANTE LA INVOCACIÓN: {ex.InnerException?.Message ?? ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR DE REFLECTION: {ex.Message}");
                return null;
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
