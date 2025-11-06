using SistemaUniversidad.Personas.Interfaz;
using SistemaUniversidad.Sistema.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Personas
{

    public abstract partial class Persona : IIdentificable
    {
        // Campos privados para Encapsulación
        private string identificacion;
        private string nombre;
        private string apellido;
        private DateTime fechaNacimiento;

        
        public string Identificacion
        {
            get => identificacion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La identificación no puede estar vacía.");
                identificacion = value;
            }
        }

        public string Nombre
        {
            get => nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre no puede estar vacío.");
                nombre = value;
            }
        }

        public string Apellido
        {
            get => apellido;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El apellido no puede estar vacío.");
                apellido = value;
            }
        }

        
        public DateTime FechaNacimiento
        {
            get => fechaNacimiento;
            protected set
            {
                
                fechaNacimiento = value;
            }
        }

        
        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
               
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        // Constructor 
        public Persona(string identificacion, string nombre, string apellido, DateTime fechaNacimiento)
        {
            Identificacion = identificacion;
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
        }

     
        public abstract string ObtenerRol();

        
        public override string ToString()
        {
            return $"[{ObtenerRol()}] | ID: {Identificacion} | Nombre: {Nombre} {Apellido} | Nacimiento: {FechaNacimiento:yyyy-MM-dd} | Edad: {Edad} años";
        }
    }

    // HERENCIA Estudiante y Persona
    public class Estudiante : Persona
    {
        private const int EdadMinimaEstudiante = 15;
        [Requerido]
        public string Carrera { get; set; }
        public string NumeroMatricula { get; set; }

        public Estudiante(string identificacion, string nombre, string apellido, DateTime fechaNacimiento, string carrera, string numeroMatricula)
            : base(identificacion, nombre, apellido, fechaNacimiento)
        {
            // Validación de Edad Mínima
            if (Edad < EdadMinimaEstudiante)
                throw new ArgumentException($"Edad: {Edad}. La edad mínima para ser estudiante es {EdadMinimaEstudiante} años.");

            Carrera = carrera;
            NumeroMatricula = numeroMatricula;
        }

        // Implementación del método abstracto
        public override string ObtenerRol()
        {
            return "Estudiante";
        }

        // Sobrescritura de ToString() (opcional, mejora la visualización)
        public override string ToString()
        {
            return base.ToString() + $" | Carrera: {Carrera} | Matrícula: {NumeroMatricula}";
        }
    }


    public class Profesor : Persona
    {
        private const int EdadMinimaProfesor = 25; // Requisito de validación
        
        [Requerido]
        public string Departamento { get; set; }
        public TipoContrato TipoContrato { get; set; }
       
        [ValidacionRango(1000.00, 100000.00)]

        public decimal SalarioBase { get; set; }

        public Profesor(string identificacion, string nombre, string apellido, DateTime fechaNacimiento, string departamento, TipoContrato tipoContrato, decimal salarioBase)
            : base(identificacion, nombre, apellido, fechaNacimiento)
        {
            // Validación de Edad Mínima
            if (Edad < EdadMinimaProfesor)
                throw new ArgumentException($"Edad: {Edad}. La edad mínima para ser profesor es {EdadMinimaProfesor} años.");

            Departamento = departamento;
            TipoContrato = tipoContrato;
            SalarioBase = salarioBase;
        }

        // Implementación del método abstracto
        public override string ObtenerRol()
        {
            return "Profesor";
        }

        // Sobrescritura de ToString() (opcional, mejora la visualización)
        public override string ToString()
        {
            return base.ToString() + $" | Dpto: {Departamento} | Contrato: {TipoContrato} | Salario: {SalarioBase:C}";
        }
    }
}
