using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Sistema.Atributos
{

    // --- Atributo Requerido ---
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequeridoAttribute : Attribute { }

    // --- Atributo ValidacionRango (Corregido) ---
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidacionRangoAttribute : Attribute
    {
        // Las propiedades internas que usa el Validador siguen siendo decimales
        public decimal Minimo { get; }
        public decimal Maximo { get; }

        // ¡CORRECCIÓN AQUÍ! Los parámetros del constructor deben ser double
        public ValidacionRangoAttribute(double min, double max)
        {
            // Convertimos de double a decimal para almacenar los valores
            Minimo = (decimal)min;
            Maximo = (decimal)max;
        }
    }

    // --- Atributo Formato ---
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FormatoAttribute : Attribute
    {
        public string PatronRegex { get; }
        public FormatoAttribute(string patronRegex) { PatronRegex = patronRegex; }
    }

}
