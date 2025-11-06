using SistemaUniversidad.Personas;
using SistemaUniversidad.Personas.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace SistemaUniversidad.Sistema
{
    public class GestorMatriculas
    {
        // Almacenamiento central de matrículas: 
        private readonly Dictionary<string, Matricula> _matriculas = new Dictionary<string, Matricula>();

        // Dependencias para buscar entidades
        private readonly Repositorio<Estudiante> _repoEstudiantes;
        private readonly Repositorio<Curso> _repoCursos;

        public GestorMatriculas(Repositorio<Estudiante> repoEstudiantes, Repositorio<Curso> repoCursos)
        {
            _repoEstudiantes = repoEstudiantes ?? throw new ArgumentNullException(nameof(repoEstudiantes));
            _repoCursos = repoCursos ?? throw new ArgumentNullException(nameof(repoCursos));
        }

        // Helper para generar la clave única
        private string GenerarClaveMatricula(string idEstudiante, string codigoCurso)
        {
            return $"{idEstudiante}-{codigoCurso}".ToUpperInvariant();
        }

        // --- Método Auxiliar LINQ (Necesario para las consultas) ---
        // Calcula el promedio de promedios de todos los cursos de un estudiante.
        private decimal ObtenerPromedioGeneralEstudiante(string idEstudiante)
        {
            var matriculas = ObtenerMatriculasPorEstudiante(idEstudiante);

            var promediosValidos = matriculas
                .Where(m => m.Calificaciones.Any())
                .Select(m => m.ObtenerPromedio());

            return promediosValidos.Any() ? promediosValidos.Average() : 0m;
        }

        // --- MÉTODOS DE GESTIÓN (Mantenidos de la estructura original) ---

        public Matricula MatricularEstudiante(string idEstudiante, string codigoCurso)
        {
            var estudiante = _repoEstudiantes.BuscarPorId(idEstudiante) as Estudiante;
            var curso = _repoCursos.BuscarPorId(codigoCurso) as Curso;

            if (estudiante == null) throw new ArgumentException($"Estudiante con ID '{idEstudiante}' no encontrado.");
            if (curso == null) throw new ArgumentException($"Curso con código '{codigoCurso}' no encontrado.");

            string clave = GenerarClaveMatricula(idEstudiante, codigoCurso);
            if (_matriculas.ContainsKey(clave)) throw new InvalidOperationException($"El estudiante ya está matriculado en el curso {curso.Nombre}.");

            var nuevaMatricula = new Matricula(estudiante, curso, DateTime.Today);
            _matriculas.Add(clave, nuevaMatricula);
            return nuevaMatricula;
        }

        public void AgregarCalificacion(string idEstudiante, string codigoCurso, decimal calificacion)
        {
            if (calificacion < 0m || calificacion > 10m)
                throw new ArgumentOutOfRangeException(nameof(calificacion), "La calificación debe estar entre 0 y 10.");

            string clave = GenerarClaveMatricula(idEstudiante, codigoCurso);
            if (!_matriculas.TryGetValue(clave, out Matricula matricula))
            {
                throw new ArgumentException("No se encontró una matrícula activa para el estudiante en este curso.");
            }
            matricula.AgregarCalificacion(calificacion);
        }

        public List<Matricula> ObtenerMatriculasPorEstudiante(string idEstudiante)
        {
            return _matriculas.Values
                .Where(m => m.Estudiante.Identificacion.Equals(idEstudiante, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Estudiante> ObtenerEstudiantesPorCurso(string codigoCurso)
        {
            return _matriculas.Values
                .Where(m => m.Curso.Identificacion.Equals(codigoCurso, StringComparison.OrdinalIgnoreCase))
                .Select(m => m.Estudiante)
                .Distinct()
                .ToList();
        }

        public string GenerarReporteEstudiante(string idEstudiante)
        {
            var matriculas = ObtenerMatriculasPorEstudiante(idEstudiante);
            if (!matriculas.Any()) return $"--- Reporte para ID {idEstudiante} ---\nNo se encontraron matrículas para este estudiante.";
            // ... (Resto de la implementación del reporte)
            var sb = new System.Text.StringBuilder();
            var estudiante = matriculas.First().Estudiante;
            sb.AppendLine($"--- REPORTE ACADÉMICO: {estudiante.Nombre} {estudiante.Apellido} ---");
            sb.AppendLine($"ID: {idEstudiante} | Carrera: {estudiante.Carrera}");
            sb.AppendLine(new string('-', 50));
            foreach (var mat in matriculas)
            {
                sb.AppendLine($"Curso: {mat.Curso.Nombre}");
                sb.AppendLine($"  Promedio: {mat.ObtenerPromedio():F2} | Estado: **{mat.ObtenerEstado()}**");
            }
            return sb.ToString();
        }

        // =====================================================================
        // IMPLEMENTACIÓN DE CONSULTAS LINQ Y LAMBDA
        // =====================================================================

        // 1. ObtenerTop10Estudiantes() - Top 10 por promedio general
        public List<(Estudiante Estudiante, decimal Promedio)> ObtenerTop10Estudiantes()
        {
            var estudiantesMatriculados = _matriculas.Values
                .Select(m => m.Estudiante)
                .Distinct();

            return estudiantesMatriculados
                .Select(e => new
                {
                    Estudiante = e,
                    Promedio = ObtenerPromedioGeneralEstudiante(e.Identificacion)
                })
                .Where(x => x.Promedio > 0) // Solo estudiantes con calificaciones
                .OrderByDescending(x => x.Promedio) // Ordenar de mayor a menor
                .Take(10)
                .Select(x => (x.Estudiante, x.Promedio))
                .ToList();
        }

        // 2. ObtenerEstudiantesEnRiesgo() - Promedio < 7.0
        public List<(Estudiante Estudiante, decimal Promedio)> ObtenerEstudiantesEnRiesgo(decimal limitePromedio = 7.0m)
        {
            var estudiantesMatriculados = _matriculas.Values
                .Select(m => m.Estudiante)
                .Distinct();

            return estudiantesMatriculados
                .Select(e => new
                {
                    Estudiante = e,
                    Promedio = ObtenerPromedioGeneralEstudiante(e.Identificacion)
                })
                // Filtrar por promedio > 0 Y promedio < 7.0
                .Where(x => x.Promedio > 0m && x.Promedio < limitePromedio)
                .OrderBy(x => x.Promedio) // Ordenar del más bajo al más alto
                .Select(x => (x.Estudiante, x.Promedio))
                .ToList();
        }

        // 3. ObtenerCursosMasPopulares() - Ordenados por cantidad de estudiantes
        public List<(Curso Curso, int Conteo)> ObtenerCursosMasPopulares()
        {
            return _matriculas.Values
                // Agrupar por el objeto Curso
                .GroupBy(m => m.Curso)
                .Select(g => new
                {
                    Curso = g.Key,
                    Conteo = g.Select(m => m.Estudiante).Distinct().Count() // Contar estudiantes únicos
                })
                .OrderByDescending(x => x.Conteo)
                .Select(x => (x.Curso, x.Conteo))
                .ToList();
        }

        // 4. ObtenerPromedioGeneral() - Promedio de todos los estudiantes
        public decimal ObtenerPromedioGeneral()
        {
            var promediosEstudiantes = _matriculas.Values
                .Select(m => m.Estudiante)
                .Distinct()
                .Select(e => ObtenerPromedioGeneralEstudiante(e.Identificacion))
                .Where(p => p > 0m); // Excluir estudiantes sin calificaciones

            return promediosEstudiantes.Any() ? promediosEstudiantes.Average() : 0m;
        }

        // 5. ObtenerEstadisticasPorCarrera() - Agrupar y mostrar: cantidad, promedio
        public List<object> ObtenerEstadisticasPorCarrera()
        {
            var estadisticas = _matriculas.Values
                .Select(m => m.Estudiante)
                .Distinct()
                // Agrupar por la propiedad Carrera
                .GroupBy(e => e.Carrera)
                .Select(g =>
                {
                    var promediosCarrera = g.Select(e => ObtenerPromedioGeneralEstudiante(e.Identificacion))
                                            .Where(p => p > 0m);

                    return new
                    {
                        Carrera = g.Key,
                        CantidadEstudiantes = g.Count(),
                        PromedioGeneral = promediosCarrera.Any() ? promediosCarrera.Average() : 0m
                    };
                })
                .OrderByDescending(x => x.PromedioGeneral)
                .Cast<object>()
                .ToList();

            return estadisticas;
        }

        // 6. BuscarEstudiantes(Func<Estudiante, bool> criterio) - Búsqueda flexible con predicado
        public List<Estudiante> BuscarEstudiantes(Func<Estudiante, bool> criterio)
        {
            // Obtener la colección única de estudiantes matriculados
            var todosEstudiantes = _matriculas.Values
                .Select(m => m.Estudiante)
                .Distinct();

            // Aplicar el Delegado Func<T, bool> (criterio/Lambda) para filtrar
            return todosEstudiantes
                .Where(criterio)
                .ToList();
        }
    }
}