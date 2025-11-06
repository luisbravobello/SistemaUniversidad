using SistemaUniversidad.Personas.Interfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Personas
{

    // Clase Matricula e Interfaz IEvaluable
    public class Matricula : IEvaluable
    {
        private const decimal CalificacionMinimaAprobacion = 7.0m;

        public Estudiante Estudiante { get; private set; }
        public Curso Curso { get; private set; }
        public DateTime FechaMatricula { get; private set; }

        public List<decimal> Calificaciones { get; private set; }

        public Matricula(Estudiante estudiante, Curso curso, DateTime fechaMatricula)
        {
            Estudiante = estudiante ?? throw new ArgumentNullException(nameof(estudiante));
            Curso = curso ?? throw new ArgumentNullException(nameof(curso));
            FechaMatricula = fechaMatricula;
            Calificaciones = new List<decimal>();
        }

      

        public void AgregarCalificacion(decimal calificacion)
        {
            
            if (calificacion < 0m || calificacion > 10m)
                throw new ArgumentOutOfRangeException(nameof(calificacion), "La calificación debe estar entre 0 y 10.");

            Calificaciones.Add(calificacion);
        }

        public decimal ObtenerPromedio()
        {
           
            if (!Calificaciones.Any())
                return 0m;

            return Calificaciones.Average();
        }

        public bool HaAprobado()
        {
           
            if (!Calificaciones.Any())
                return false;

            return ObtenerPromedio() >= CalificacionMinimaAprobacion;
        }


        public string ObtenerEstado()
        {
            if (!Calificaciones.Any())
            {
                return "En Curso";
            }
            else if (HaAprobado())
            {
                return "Aprobado";
            }
            else
            {
                return "Reprobado";
            }
        }

        public override string ToString()
        {
            string promedio = ObtenerPromedio().ToString("F2");
            return $"Matrícula de {Estudiante.Nombre} en {Curso.Nombre}. Promedio: {promedio}. Estado: {ObtenerEstado()}";
        }
    }
}
