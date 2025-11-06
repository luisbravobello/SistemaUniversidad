using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Personas.Interfaz
{
    public interface IIdentificable
    {
        // Propiedad de solo lectura requerida
        string Identificacion { get; }
    }
}
