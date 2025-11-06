using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Sistema.Atributos
{

    // Atributo Requerido 
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequeridoAttribute : Attribute { }

    // Atributo ValidacionRango 
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidacionRangoAttribute : Attribute
    {
       
        public decimal Minimo { get; }
        public decimal Maximo { get; }

        
        public ValidacionRangoAttribute(double min, double max)
        {
            
            Minimo = (decimal)min;
            Maximo = (decimal)max;
        }
    }

    // Atributo Formato 
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FormatoAttribute : Attribute
    {
        public string PatronRegex { get; }
        public FormatoAttribute(string patronRegex) { PatronRegex = patronRegex; }
    }

}
