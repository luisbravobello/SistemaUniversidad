using SistemaUniversidad.Personas.Interfaz;
using SistemaUniversidad.Sistema.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Personas
{
   
    public partial class Curso : IIdentificable
    {
        [Requerido]
        [Formato(@"^[A-Z]{2,3}\d{3}$")]
        public string Codigo { get; private set; }
      

        public string Identificacion => Codigo;

        [Requerido]
        public string Nombre { get;  set; }
        
        [ValidacionRango(1, 10)]
        public int Creditos { get; set; }

        public Profesor ProfesorAsignado { get; set; }

        public Curso(string codigo, string nombre, int creditos, Profesor profesorAsignado = null)
        {

            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("El código no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre no puede estar vacío.");
            if (creditos <= 0) throw new ArgumentException("Los créditos deben ser positivos.");

            // Ahora la asignación funciona
            Codigo = codigo;
            Nombre = nombre;
            Creditos = creditos;
            ProfesorAsignado = profesorAsignado;
        }

        public override string ToString()
        {
            string profesorInfo = ProfesorAsignado != null ? ProfesorAsignado.Nombre + " " + ProfesorAsignado.Apellido : "No Asignado";
            return $"[Curso {Codigo}] {Nombre} ({Creditos} Créditos). Profesor: {profesorInfo}";
        }
    }
}