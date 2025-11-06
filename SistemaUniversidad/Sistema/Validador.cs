using System;
using System.Collections.Generic;
using SistemaUniversidad.Sistema.Atributos;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SistemaUniversidad.Sistema
{
    public static class Validador
    {
        
        public static List<string> ValidarInstancia(object instancia)
        {
            var errores = new List<string>();
            if (instancia == null)
            {
                errores.Add("La instancia a validar es nula.");
                return errores;
            }

            Type tipo = instancia.GetType();

            
            PropertyInfo[] propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in propiedades)
            {
                object valor = prop.GetValue(instancia);
                string nombrePropiedad = prop.Name;

                // 1. Validar [Requerido]
                if (prop.GetCustomAttribute<RequeridoAttribute>() != null)
                {
                    if (valor == null || (valor is string s && string.IsNullOrWhiteSpace(s)))
                    {
                        errores.Add($"ERROR: '{nombrePropiedad}' es requerido y no puede estar vacío.");
                        continue;
                    }
                }

                var rangoAttr = prop.GetCustomAttribute<ValidacionRangoAttribute>();
                if (rangoAttr != null && valor != null)
                {
                   
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(int))
                    {
                        decimal valorNumerico = Convert.ToDecimal(valor);

                        if (valorNumerico < rangoAttr.Minimo || valorNumerico > rangoAttr.Maximo)
                        {
                            errores.Add($"ERROR: '{nombrePropiedad}' ({valorNumerico}) debe estar entre {rangoAttr.Minimo} y {rangoAttr.Maximo}.");
                        }
                    }
                }

                
                var formatoAttr = prop.GetCustomAttribute<FormatoAttribute>();
                if (formatoAttr != null && valor is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
                {
                    // Usar Regex para validar el patrón
                    if (!Regex.IsMatch(stringValue, formatoAttr.PatronRegex))
                    {
                        errores.Add($"ERROR: '{nombrePropiedad}' ('{stringValue}') no cumple con el formato requerido ({formatoAttr.PatronRegex}).");
                    }
                }
            }

            return errores;
        }
    }
}
