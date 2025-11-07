using SistemaUniversidad.Sistema.Atributos;
using SistemaUniversidad.Personas;
using SistemaUniversidad.Sistema.Reflection;
using SistemaUniversidad.Personas.Repositorio;
using SistemaUniversidad.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



public class ProgramaPrincipal
{
    // Instancias de los gestores y repositorios
    private static readonly Repositorio<Estudiante> _repoEstudiantes = new Repositorio<Estudiante>();
    private static readonly Repositorio<Profesor> _repoProfesores = new Repositorio<Profesor>();
    private static readonly Repositorio<Curso> _repoCursos = new Repositorio<Curso>();
    private static readonly GestorMatriculas _gestorMatriculas;

    static ProgramaPrincipal()
    {
       
        _gestorMatriculas = new GestorMatriculas(_repoEstudiantes, _repoCursos);
        CargarDatosDePruebaIniciales();
    }

    public static void Main(string[] args)
    {
        bool continuar = true;
   
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        MostrarAsciiUniversidad();
        PausarConsola();
  

        while (continuar)
        {
            MostrarMenuPrincipal();
            string opcion = Console.ReadLine()?.ToUpper();

            try
            {
                switch (opcion)
                {
                    case "1": GestionarEstudiantes(); break;
                    case "2": GestionarProfesores(); break;
                    case "3": GestionarCursos(); break;
                    case "4": MatricularEstudianteEnCurso(); PausarConsola(); break;
                    case "5": RegistrarCalificaciones(); PausarConsola(); break;
                    case "6": VerReportes(); PausarConsola(); break;
                    case "7": AnalisisReflection(); PausarConsola(); break;
                    case "S":
                    case "8":
                        continuar = false;
                        
                        MostrarMensaje("Saliendo del sistema. ¡Hasta pronto!", ConsoleColor.Yellow);
                        break;
                    default:
                        MostrarMensaje("Opción no válida. Intente de nuevo.", ConsoleColor.Red);
                        PausarConsola();
                        break;
                }
            }
            catch (Exception ex)
            {
                
                MostrarMensaje($"\nERROR INESPERADO: {ex.Message}", ConsoleColor.Red);
                PausarConsola();
            }
        }
    }

    // MÉTODOS DE UI Y UTILIDAD

    private static void MostrarAsciiUniversidad()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("|    Universidad Central del Este (UCE)           |");
        Console.WriteLine("--------------------------------------------------");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"");
        Console.WriteLine(@"          ___          ___");
        Console.WriteLine(@"         |   |        |   |");
        Console.WriteLine(@"         | U |________| C |");
        Console.WriteLine(@"         |   |  -==-  |   |");
        Console.WriteLine(@"         |___| |####| |___|");
        Console.WriteLine(@"         | - | |####| | - |");
        Console.WriteLine(@"         |___| |####| |___|");
        Console.WriteLine(@"         /  U C E  |  \ S G A \");
        Console.WriteLine(@"        /___________|___________\");
        Console.ResetColor();
    }

    
    private static void MostrarMenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine("------------------------------------------");
        MostrarMensaje("      SISTEMA DE GESTIÓN ACADÉMICA UCE      ", ConsoleColor.Cyan);
        Console.WriteLine("------------------------------------------");
        MostrarMensaje("1. Gestionar Estudiantes ", ConsoleColor.White);
        MostrarMensaje("2. Gestionar Profesores ", ConsoleColor.White);
        MostrarMensaje("3. Gestionar Cursos ", ConsoleColor.White);
        MostrarMensaje("4. Matricular Estudiante en Curso", ConsoleColor.White);
        MostrarMensaje("5. Registrar Calificaciones", ConsoleColor.White);
        MostrarMensaje("6. Ver Reportes", ConsoleColor.White);
        MostrarMensaje("7. Análisis con Reflection", ConsoleColor.White);
        MostrarMensaje("8. Salir", ConsoleColor.Red);
        Console.WriteLine("--------------------------------------------");
        Console.Write("Seleccione una opción: ");
        Console.ResetColor();
    }

    private static void MostrarMensaje(string mensaje, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(mensaje);
        Console.ResetColor();
    }

    private static void PausarConsola()
    {
        Console.Write("\nPresione cualquier tecla para continuar...");
        Console.ReadKey(true);
    }

    private static string SolicitarEntrada(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    // 1. GESTIÓN DE ESTUDIANTE
 
    private static void GestionarEstudiantes()
    {
        bool seguirGestionando = true;
        while (seguirGestionando) // ESTE ES EL BUCLE CONTINUO
        {
            Console.Clear();
            MostrarMensaje("--- GESTIÓN DE ESTUDIANTES  ---", ConsoleColor.Blue);
            Console.WriteLine("1. Agregar Nuevo Estudiante "); 
            Console.WriteLine("2. Listar Todos los Estudiantes "); 
            Console.WriteLine("3. Buscar Estudiante por ID "); 
            Console.WriteLine("4. Modificar Estudiante "); 
            Console.WriteLine("5. Eliminar Estudiante ");
            MostrarMensaje("6. <- Regresar al Menú Principal", ConsoleColor.DarkGray);
            Console.WriteLine(new string('-', 40));
            Console.Write("Opción de gestión: ");

            if (int.TryParse(Console.ReadLine(), out int opcion))
            {
                try
                {
                    switch (opcion)
                    {
                        case 1: AgregarEstudiante(); break;
                        case 2: ListarEstudiantes(false); break;
                        case 3: BuscarEstudiante(); break;
                        case 4: ModificarEstudiante(); break;
                        case 5: EliminarEstudiante(); break;
                        case 6: seguirGestionando = false; break; // Rompe el sub-bucle
                        default: MostrarMensaje("Opción no válida.", ConsoleColor.Red); break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error de Gestión: {ex.Message}", ConsoleColor.Red);
                }
                if (seguirGestionando) PausarConsola();
            }
        }
    }

    private static void AgregarEstudiante()
    {
        MostrarMensaje("\n--- AGREGAR ESTUDIANTE ---", ConsoleColor.Green);
        try
        {
            string id = SolicitarEntrada("ID (Ej: E123): ");
            string nombre = SolicitarEntrada("Nombre: ");
            string apellido = SolicitarEntrada("Apellido: ");
            string fechaStr = SolicitarEntrada("Fecha Nacimiento (AAAA-MM-DD):  ");
            DateTime fechaNac = DateTime.Parse(fechaStr);
            string carrera = SolicitarEntrada("Carrera: ");
            string matricula = SolicitarEntrada("Matrícula (Ej: M2024123): ");

            var nuevoEstudiante = new Estudiante(id, nombre, apellido, fechaNac, carrera, matricula);

            var errores = Validador.ValidarInstancia(nuevoEstudiante);
            if (errores.Any())
            {
                MostrarMensaje("Fallo de Validación por Atributos:", ConsoleColor.Red);
                errores.ForEach(e => Console.WriteLine($" - {e}"));
                return;
            }
            _repoEstudiantes.Agregar(nuevoEstudiante);
           
            MostrarMensaje($"Estudiante {nombre} {apellido} agregado con éxito.", ConsoleColor.Green);
        }
        catch (ArgumentException ex)
        {
            MostrarMensaje($"Error de Validación de Datos (Ej: Edad, ID duplicada): {ex.Message}", ConsoleColor.Red);
        }
        catch (FormatException)
        {
            MostrarMensaje("Error: Formato de fecha o datos inválido.", ConsoleColor.Red);
        }
    }

    private static void ListarEstudiantes(bool pausa)
    {
        MostrarMensaje("\n--- LISTADO DE ESTUDIANTES ---", ConsoleColor.Yellow);
        var estudiantes = _repoEstudiantes.ObtenerTodos().Cast<Estudiante>().ToList();

        if (estudiantes.Any())
        {
            Console.WriteLine("ID | Nombre Completo | Edad | Carrera | Matrícula");
            Console.WriteLine(new string('-', 70));
            foreach (var e in estudiantes)
            {
                Console.WriteLine($"{e.Identificacion} | {e.Nombre} {e.Apellido} | {e.Edad} | {e.Carrera} | {e.NumeroMatricula}");
            }
            Console.WriteLine(new string('-', 70));
        }
        else
        {
            MostrarMensaje("No hay estudiantes registrados.", ConsoleColor.Yellow);
        }
    }

    private static Estudiante BuscarEstudiante()
    {
        MostrarMensaje("\n--- BUSCAR ESTUDIANTE ---", ConsoleColor.Yellow);
        string id = SolicitarEntrada("ID del Estudiante a buscar: ");

        var estudiante = _repoEstudiantes.BuscarPorId(id) as Estudiante;

        if (estudiante != null)
        {
            MostrarMensaje($"Estudiante Encontrado:", ConsoleColor.Cyan);
            Console.WriteLine(estudiante.ToString());
            return estudiante;
        }
        else
        {
            MostrarMensaje($"Error: Estudiante con ID '{id}' no encontrado.", ConsoleColor.Red);
            return null;
        }
    }

    private static void ModificarEstudiante()
    {
        MostrarMensaje("\n--- MODIFICAR ESTUDIANTE ---", ConsoleColor.Blue);
        string id = SolicitarEntrada("ID del Estudiante a modificar: ");

        var estudiante = _repoEstudiantes.BuscarPorId(id) as Estudiante;

        if (estudiante == null)
        {
            MostrarMensaje($"Error: Estudiante con ID '{id}' no encontrado.", ConsoleColor.Red);
            return;
        }

        MostrarMensaje($"Modificando Estudiante: {estudiante.Nombre} {estudiante.Apellido}", ConsoleColor.Cyan);

        string nuevoNombre = SolicitarEntrada($"Nombre actual ({estudiante.Nombre}): ");
        if (!string.IsNullOrWhiteSpace(nuevoNombre)) estudiante.Nombre = nuevoNombre;

        string nuevaCarrera = SolicitarEntrada($"Carrera actual ({estudiante.Carrera}): ");
        if (!string.IsNullOrWhiteSpace(nuevaCarrera)) estudiante.Carrera = nuevaCarrera;

        string nuevaMatricula = SolicitarEntrada($"Matrícula actual ({estudiante.NumeroMatricula}): ");
        if (!string.IsNullOrWhiteSpace(nuevaMatricula)) estudiante.NumeroMatricula = nuevaMatricula;

        var errores = Validador.ValidarInstancia(estudiante);
        if (errores.Any())
        {
            MostrarMensaje("Fallo de Validación por Atributos (Cambios no guardados):", ConsoleColor.Red);
            errores.ForEach(e => Console.WriteLine($" - {e}"));
            return;
        }

        
        MostrarMensaje($"Estudiante con ID {id} modificado exitosamente.", ConsoleColor.Blue);
    }

    private static void EliminarEstudiante()
    {
        MostrarMensaje("\n--- ELIMINAR ESTUDIANTE ---", ConsoleColor.Red);
        string id = SolicitarEntrada("ID del Estudiante a eliminar: ");

        try
        {
            _repoEstudiantes.Eliminar(id);
            
            MostrarMensaje($"Estudiante con ID {id} eliminado exitosamente.", ConsoleColor.Red);
        }
        catch (KeyNotFoundException)
        {
            MostrarMensaje($"Error: No se puede eliminar. Estudiante con ID '{id}' no encontrado.", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al eliminar: {ex.Message}", ConsoleColor.Red);
        }
    }

    // 2. GESTIÓN DE PROFESORES 

    private static void GestionarProfesores()
    {
        bool seguirGestionando = true;
        while (seguirGestionando) 
        {
            Console.Clear();
            MostrarMensaje("--- GESTIÓN DE PROFESORES  ---", ConsoleColor.Blue);
            Console.WriteLine("1. Agregar Nuevo Profesor ");
            Console.WriteLine("2. Listar Todos los Profesores ");
            Console.WriteLine("3. Buscar Profesor por ID ");
            Console.WriteLine("4. Modificar Profesor ");
            Console.WriteLine("5. Eliminar Profesor");
            MostrarMensaje("6. <- Regresar al Menú Principal", ConsoleColor.DarkGray);
            Console.WriteLine(new string('-', 40));
            Console.Write("Opción de gestión: ");

            if (int.TryParse(Console.ReadLine(), out int opcion))
            {
                try
                {
                    switch (opcion)
                    {
                        case 1: AgregarProfesor(); break;
                        case 2: ListarProfesores(false); break;
                        case 3: BuscarProfesor(); break;
                        case 4: ModificarProfesor(); break;
                        case 5: EliminarProfesor(); break;
                        case 6: seguirGestionando = false; break;
                        default: MostrarMensaje("Opción no válida.", ConsoleColor.Red); break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error de Gestión: {ex.Message}", ConsoleColor.Red);
                }
                if (seguirGestionando) PausarConsola();
            }
        }
    }

    // Metodo para agregar profesores

    private static void AgregarProfesor()
    {
        MostrarMensaje("\n--- AGREGAR PROFESOR ---", ConsoleColor.Green);
        try
        {
            string id = SolicitarEntrada("ID (Ej: P123): ");
            string nombre = SolicitarEntrada("Nombre: ");
            string apellido = SolicitarEntrada("Apellido: ");
            string fechaStr = SolicitarEntrada("Fecha Nacimiento (AAAA-MM-DD): ");
            DateTime fechaNac = DateTime.Parse(fechaStr);
            string departamento = SolicitarEntrada("Departamento: ");

            MostrarMensaje("Tipo de Contrato (0=TiempoCompleto, 1=MedioTiempo, 2=PorHoras):", ConsoleColor.White);
            if (!int.TryParse(SolicitarEntrada("Opción: "), out int contratoInt) || !Enum.IsDefined(typeof(TipoContrato), contratoInt))
            {
                throw new ArgumentException("Opción de contrato no válida.");
            }
            TipoContrato tipoContrato = (TipoContrato)contratoInt;

            string salarioStr = SolicitarEntrada("Salario Base: ");
            if (!decimal.TryParse(salarioStr, out decimal salarioBase))
            {
                throw new FormatException("Formato de salario inválido.");
            }

            var nuevoProfesor = new Profesor(id, nombre, apellido, fechaNac, departamento, tipoContrato, salarioBase);

            var errores = Validador.ValidarInstancia(nuevoProfesor);
            if (errores.Any())
            {
                MostrarMensaje("Fallo de Validación por Atributos:", ConsoleColor.Red);
                errores.ForEach(e => Console.WriteLine($" - {e}"));
                return;
            }

            _repoProfesores.Agregar(nuevoProfesor);
           
            MostrarMensaje($"Profesor {nombre} {apellido} agregado con éxito.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al agregar profesor: {ex.Message}", ConsoleColor.Red);
        }
    }

    // Metodo para listar profesores

    private static void ListarProfesores(bool pausa)
    {
        MostrarMensaje("\n--- LISTADO DE PROFESORES ---", ConsoleColor.Yellow);
        var profesores = _repoProfesores.ObtenerTodos().Cast<Profesor>().ToList();

        if (profesores.Any())
        {
            Console.WriteLine("ID | Nombre Completo | Departamento | Contrato | Salario Base");
            Console.WriteLine(new string('-', 70));

            foreach (var p in profesores)
            {
                Console.WriteLine($"{p.Identificacion} | {p.Nombre} {p.Apellido} | {p.Departamento} | {p.TipoContrato} | {p.SalarioBase:C}");
            }
            Console.WriteLine(new string('-', 70));
        }
        else
        {
            MostrarMensaje("No hay profesores registrados.", ConsoleColor.Yellow);
        }
    }

    // Metodo para buscar profesores

    private static Profesor BuscarProfesor()
    {
        MostrarMensaje("\n--- BUSCAR PROFESOR ---", ConsoleColor.Yellow);
        string id = SolicitarEntrada("ID del Profesor a buscar: ");

        var profesor = _repoProfesores.BuscarPorId(id) as Profesor;

        if (profesor != null)
        {
            MostrarMensaje($"Profesor Encontrado:", ConsoleColor.Cyan);
            Console.WriteLine(profesor.ToString());
            return profesor;
        }
        else
        {
            MostrarMensaje($"Error: Profesor con ID '{id}' no encontrado.", ConsoleColor.Red);
            return null;
        }
    }
   
    // Metodo para modificar profesores

    private static void ModificarProfesor()
    {
        MostrarMensaje("\n--- MODIFICAR PROFESOR ---", ConsoleColor.Blue);
        string id = SolicitarEntrada("ID del Profesor a modificar: ");

        var profesor = _repoProfesores.BuscarPorId(id) as Profesor;

        if (profesor == null)
        {
            MostrarMensaje($"Error: Profesor con ID '{id}' no encontrado.", ConsoleColor.Red);
            return;
        }

        MostrarMensaje($"Modificando Profesor: {profesor.Nombre} {profesor.Apellido}", ConsoleColor.Cyan);

        string nuevoDepartamento = SolicitarEntrada($"Departamento actual ({profesor.Departamento}): ");
        if (!string.IsNullOrWhiteSpace(nuevoDepartamento)) profesor.Departamento = nuevoDepartamento;

        string nuevoSalarioStr = SolicitarEntrada($"Salario Base actual ({profesor.SalarioBase:C}): ");
        if (decimal.TryParse(nuevoSalarioStr, out decimal nuevoSalario)) profesor.SalarioBase = nuevoSalario;

        var errores = Validador.ValidarInstancia(profesor);
        if (errores.Any())
        {
            MostrarMensaje("Fallo de Validación por Atributos (Cambios no guardados):", ConsoleColor.Red);
            return;
        }

        
        MostrarMensaje($"Profesor con ID {id} modificado exitosamente.", ConsoleColor.Blue);
    }

    // Metodo para eliminar profesores

    private static void EliminarProfesor()
    {
        MostrarMensaje("\n--- ELIMINAR PROFESOR ---", ConsoleColor.Red);
        string id = SolicitarEntrada("ID del Profesor a eliminar: ");

        try
        {
            _repoProfesores.Eliminar(id);
            
            MostrarMensaje($"Profesor con ID {id} eliminado exitosamente.", ConsoleColor.Red);
        }
        catch (KeyNotFoundException)
        {
            MostrarMensaje($"Error: No se puede eliminar. Profesor con ID '{id}' no encontrado.", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al eliminar: {ex.Message}", ConsoleColor.Red);
        }
    }

    // 3. GESTIÓN DE CURSOS 
 
    private static void GestionarCursos()
    {
        bool seguirGestionando = true;
        while (seguirGestionando)
        {
            Console.Clear();
            MostrarMensaje("--- GESTIÓN DE CURSOS  ---", ConsoleColor.Blue);
            Console.WriteLine("1. Agregar Nuevo Curso ");
            Console.WriteLine("2. Listar Todos los Cursos ");
            Console.WriteLine("3. Modificar Curso ");
            Console.WriteLine("4. Asignar Profesor ");
            Console.WriteLine("5. Eliminar Curso ");
            MostrarMensaje("6. <- Regresar al Menú Principal", ConsoleColor.DarkGray);
            Console.WriteLine(new string('-', 40));
            Console.Write("Opción de gestión: ");

            if (int.TryParse(Console.ReadLine(), out int opcion))
            {
                try
                {
                    switch (opcion)
                    {
                        case 1: AgregarCurso(); break;
                        case 2: ListarCursos(false); break;
                        case 3: ModificarCurso(); break;
                        case 4: AsignarProfesorACurso(); break;
                        case 5: EliminarCurso(); break;
                        case 6: seguirGestionando = false; break;
                        default: MostrarMensaje("Opción no válida.", ConsoleColor.Red); break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error de Gestión: {ex.Message}", ConsoleColor.Red);
                }
                if (seguirGestionando) PausarConsola();
            }
        }
    }
    
    //METODOS
    private static void AgregarCurso()
    {
        MostrarMensaje("\n--- AGREGAR CURSO ---", ConsoleColor.Green);
        try
        {
            string codigo = SolicitarEntrada("Código del Curso (Ej: CS101): ");
            string nombre = SolicitarEntrada("Nombre del Curso: ");
            string creditosStr = SolicitarEntrada("Créditos: ");

            if (!int.TryParse(creditosStr, out int creditos))
            {
                throw new FormatException("Formato de créditos inválido.");
            }

            var nuevoCurso = new Curso(codigo, nombre, creditos, null);

            var errores = Validador.ValidarInstancia(nuevoCurso);
            if (errores.Any())
            {
                MostrarMensaje("Fallo de Validación por Atributos:", ConsoleColor.Red);
                errores.ForEach(e => Console.WriteLine($" - {e}"));
                return;
            }
            _repoCursos.Agregar(nuevoCurso);
            
            MostrarMensaje($"Curso '{nombre}' agregado con éxito.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al agregar curso: {ex.Message}", ConsoleColor.Red);
        }
    }

    private static void ListarCursos(bool pausa)
    {
        Console.Clear();
        MostrarMensaje("--- LISTADO ACTUAL DE CURSOS ---", ConsoleColor.Yellow);

        var cursos = _repoCursos.ObtenerTodos().Cast<Curso>().ToList();

        if (!cursos.Any())
        {
            MostrarMensaje("No hay cursos registrados.", ConsoleColor.Yellow);
        }
        else
        {
            Console.WriteLine(new string('-', 70));
            Console.WriteLine("CÓDIGO | CRÉDITOS | NOMBRE DEL CURSO | PROFESOR ASIGNADO");
            Console.WriteLine(new string('-', 70));

            foreach (var c in cursos)
            {
                string profesorNombre = c.ProfesorAsignado != null
                                             ? $"{c.ProfesorAsignado.Nombre} {c.ProfesorAsignado.Apellido}"
                                             : "N/A";
                Console.WriteLine($"{c.Codigo,-6} | {c.Creditos,-8} | {c.Nombre,-18} | {profesorNombre}");
            }
            Console.WriteLine(new string('-', 70));
        }
    }

    private static void ModificarCurso()
    {
        MostrarMensaje("\n--- MODIFICAR CURSO ---", ConsoleColor.Blue);
        string codigo = SolicitarEntrada("Código del Curso a modificar: ");

        var curso = _repoCursos.BuscarPorId(codigo) as Curso;

        if (curso == null)
        {
            MostrarMensaje($"Error: Curso con código '{codigo}' no encontrado.", ConsoleColor.Red);
            return;
        }

        MostrarMensaje($"Modificando Curso: {curso.Nombre}", ConsoleColor.Cyan);

        string nuevoNombre = SolicitarEntrada($"Nombre actual ({curso.Nombre}): ");
        if (!string.IsNullOrWhiteSpace(nuevoNombre)) curso.Nombre = nuevoNombre;

        string creditosStr = SolicitarEntrada($"Créditos actuales ({curso.Creditos}): ");

        if (int.TryParse(creditosStr, out int nuevosCreditos))
        {
            curso.Creditos = nuevosCreditos;
        }
        else if (!string.IsNullOrWhiteSpace(creditosStr))
        {
            MostrarMensaje("Advertencia: Créditos no modificados. Ingrese un número entero válido.", ConsoleColor.Yellow);
        }

        var errores = Validador.ValidarInstancia(curso);
        if (errores.Any())
        {
            MostrarMensaje("Fallo de Validación por Atributos (Cambios no guardados):", ConsoleColor.Red);
            errores.ForEach(e => Console.WriteLine($" - {e}"));
            return;
        }

        
        MostrarMensaje($"Curso con código {codigo} modificado exitosamente.", ConsoleColor.Blue);
    }

    private static void AsignarProfesorACurso()
    {
        MostrarMensaje("\n--- ASIGNAR PROFESOR A CURSO ---", ConsoleColor.Blue);
        string codigoCurso = SolicitarEntrada("Código del Curso: ");
        string idProfesor = SolicitarEntrada("ID del Profesor a asignar  ");

        var curso = _repoCursos.BuscarPorId(codigoCurso) as Curso;
        if (curso == null)
        {
            MostrarMensaje($"Error: Curso con código '{codigoCurso}' no encontrado.", ConsoleColor.Red);
            return;
        }

        Profesor profesor = null;
        string mensajeAsignacion = "quitado";

        if (!string.IsNullOrWhiteSpace(idProfesor))
        {
            profesor = _repoProfesores.BuscarPorId(idProfesor) as Profesor;
            if (profesor == null)
            {
                MostrarMensaje($"Error: Profesor con ID '{idProfesor}' no encontrado.", ConsoleColor.Red);
                return;
            }
            mensajeAsignacion = $"asignado ({profesor.Nombre})";
        }

        curso.ProfesorAsignado = profesor;
        
        MostrarMensaje($"Profesor {mensajeAsignacion} al curso {curso.Nombre}.", ConsoleColor.Blue);
    }

    private static void EliminarCurso()
    {
        MostrarMensaje("\n--- ELIMINAR CURSO ---", ConsoleColor.Red);
        string id = SolicitarEntrada("Código del Curso a eliminar: ");

        try
        {
            _repoCursos.Eliminar(id);
           
            MostrarMensaje($"Curso con código {id} eliminado exitosamente.", ConsoleColor.Red);
        }
        catch (KeyNotFoundException)
        {
            MostrarMensaje($"Error: No se puede eliminar. Curso con código '{id}' no encontrado.", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al eliminar: {ex.Message}", ConsoleColor.Red);
        }
    }

    // 4. MÉTODOS DEL MENÚ PRINCIPAL 

    private static void MatricularEstudianteEnCurso()
    {
        Console.Clear();
        MostrarMensaje("--- MATRICULAR ESTUDIANTE ---", ConsoleColor.Green);
        string idEstudiante = SolicitarEntrada("ID Estudiante: ");
        string codigoCurso = SolicitarEntrada("Código Curso: ");

        try
        {
            _gestorMatriculas.MatricularEstudiante(idEstudiante, codigoCurso);
           
            MostrarMensaje("Matrícula realizada con éxito.", ConsoleColor.Green);
        }
        catch (InvalidOperationException ex)
        {
            MostrarMensaje($"Error de Matrícula: {ex.Message}", ConsoleColor.Yellow);
        }
        catch (ArgumentException ex)
        {
            MostrarMensaje($"Error de Entidad: {ex.Message}", ConsoleColor.Red);
        }
    }

    private static void RegistrarCalificaciones()
    {
        Console.Clear();
        MostrarMensaje("--- REGISTRAR CALIFICACIÓN ---", ConsoleColor.Blue);

        string idEstudiante = SolicitarEntrada("ID Estudiante: ");
        string codigoCurso = SolicitarEntrada("Código Curso: ");
        string entradaCalificacion = SolicitarEntrada("Calificación (0-10): ");

        decimal? calificacion = TiposEspeciales.ParsearCalificacion(entradaCalificacion);

        if (!calificacion.HasValue)
        {
            MostrarMensaje("Calificación inválida o fuera de rango (0-10).", ConsoleColor.Red);
            return;
        }

        try
        {
            _gestorMatriculas.AgregarCalificacion(idEstudiante, codigoCurso, calificacion.Value);
          
            MostrarMensaje($"Calificación {calificacion.Value} registrada con éxito.", ConsoleColor.Blue);

            
            object objCalificacion = calificacion.Value; // Boxing
            decimal calDesboxeada = (decimal)objCalificacion; // Unboxing
            Console.WriteLine($"[DEBUG P5] Calificación boxeda/desboxeada: {calDesboxeada}");
        }
        catch (ArgumentException ex)
        {
            MostrarMensaje($"Error al registrar: {ex.Message}", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error inesperado: {ex.Message}", ConsoleColor.Red);
        }
    }

    private static void VerReportes()
    {
        Console.Clear();
        MostrarMensaje("--- REPORTES Y ESTADÍSTICAS (LINQ) ---", ConsoleColor.Magenta);

        
        string idEstudiante = SolicitarEntrada("Ingrese ID del Estudiante para Reporte: ");
        Console.WriteLine("\n" + _gestorMatriculas.GenerarReporteEstudiante(idEstudiante));

        MostrarMensaje("\n** TOP 10 ESTUDIANTES **", ConsoleColor.Cyan);
        _gestorMatriculas.ObtenerTop10Estudiantes().ForEach(item =>
            Console.WriteLine($"- {item.Estudiante.Nombre} {item.Estudiante.Apellido} | Promedio: {item.Promedio:F2}")
        );

        MostrarMensaje("\n** ESTADÍSTICAS POR CARRERA **", ConsoleColor.Cyan);
        _gestorMatriculas.ObtenerEstadisticasPorCarrera().Cast<dynamic>().ToList().ForEach(stat =>
            Console.WriteLine($"- {stat.Carrera} | Cantidad: {stat.CantidadEstudiantes} | Promedio: {stat.PromedioGeneral:F2}")
        );
    }

    private static void AnalisisReflection()
    {
        Console.Clear();
        MostrarMensaje("--- ANÁLISIS DE TIPOS CON REFLECTION ---", ConsoleColor.Magenta);

        MostrarMensaje("\nANÁLISIS: Estudiante", ConsoleColor.Yellow);
        AnalizadorReflection.MostrarPropiedades(typeof(Estudiante));
        AnalizadorReflection.MostrarMetodos(typeof(Estudiante));

        MostrarMensaje("\nDemostración de Creación Dinámica de Curso:", ConsoleColor.Cyan);
        object[] paramsCurso = new object[] { "DYN01", "Curso Dinámico", 3, null };
        object cursoDinamico = AnalizadorReflection.CrearInstanciaDinamica(typeof(Curso), paramsCurso);

        if (cursoDinamico is Curso c)
        {
            MostrarMensaje($"Instancia creada: {c.Codigo} - {c.Nombre}", ConsoleColor.Green);
        }
        else
        {
            MostrarMensaje("Fallo al crear instancia dinámica.", ConsoleColor.Red);
        }
    }

    // 5. DATOS DE PRUEBA
 
    private static void CargarDatosDePruebaIniciales()
    {
        var random = new Random();

        string[] nombres = { "Ana", "Luis", "Marta", "David", "Sofía", "Carlos", "Elena", "Pedro", "Laura", "Javier", "Diana", "Miguel", "Andrea", "Samuel", "Valeria" };
        string[] apellidos = { "Gómez", "Pérez", "Rojas", "López", "Mora", "Castro", "Vargas", "Ruiz", "Hernández", "Díaz", "Navarro", "Flores" };
        string[] carreras = { "Ing. Sistemas", "Literatura", "Arquitectura", "Biología", "Matemáticas", "Derecho", "Medicina" };
        string[] departamentos = { "Tecnología", "Humanidades", "Ciencias Exactas", "Ciencias de la Salud", "Diseño" };

        int contadorProfesores = 1;
        int contadorEstudiantes = 1;
        int contadorCursos = 1;

        var profesores = new List<Profesor>();
        var estudiantes = new List<Estudiante>();
        var cursos = new List<Curso>();

        try
        {
            // 1. GENERAR PROFESORES
            for (int i = 0; i < 5; i++)
            {
                var p = new Profesor(
                    $"P{contadorProfesores++}",
                    nombres[i],
                    apellidos[random.Next(apellidos.Length)],
                    new DateTime(1980 - i, 5, 1),
                    departamentos[i % 4],
                    (TipoContrato)(i % 3),
                    50000m + random.Next(30000)
                );
                _repoProfesores.Agregar(p);
                profesores.Add(p);
            }

            // 2. GENERAR ESTUDIANTES
            for (int i = 0; i < 15; i++)
            {
                int anoNac = 2005 - random.Next(5);
                var e = new Estudiante(
                    $"E{contadorEstudiantes++}",
                    nombres[i % nombres.Length],
                    apellidos[random.Next(apellidos.Length)],
                    new DateTime(anoNac, random.Next(1, 12), random.Next(1, 28)),
                    carreras[i % carreras.Length],
                    $"M2024{contadorEstudiantes - 1:D2}"
                );
                _repoEstudiantes.Agregar(e);
                estudiantes.Add(e);
            }

            // 3. GENERAR CURSOS
            for (int i = 0; i < 10; i++)
            {
                string codigo = $"CS{contadorCursos++}";
                var c = new Curso(
                    codigo,
                    $"CS: Intro a {carreras[random.Next(carreras.Length)].Split(' ')[0]}",
                    random.Next(3, 6),
                    profesores[random.Next(profesores.Count)]
                );
                _repoCursos.Agregar(c);
                cursos.Add(c);
            }

            // 4. GENERAR MATRÍCULAS Y CALIFICACIONES
            int matriculasRealizadas = 0;
            while (matriculasRealizadas < 35)
            {
                if (!estudiantes.Any() || !cursos.Any()) break;

                var est = estudiantes[random.Next(estudiantes.Count)];
                var cur = cursos[random.Next(cursos.Count)];

                try
                {
                    _gestorMatriculas.MatricularEstudiante(est.Identificacion, cur.Identificacion);
                    matriculasRealizadas++;

                    int numCalificaciones = random.Next(3, 5);
                    for (int k = 0; k < numCalificaciones; k++)
                    {
                        decimal cal = Math.Round((decimal)(random.NextDouble() * 5.0 + 5.0), 1);
                        _gestorMatriculas.AgregarCalificacion(est.Identificacion, cur.Identificacion, cal);
                    }
                }
                catch (InvalidOperationException) { }
                catch (ArgumentException) { }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR DE CARGA DE DATOS] Fallo durante la inicialización: {ex.Message}");
            Console.ResetColor();
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n--- Carga de datos de prueba finalizada. ---");
            Console.WriteLine($"Total de estudiantes cargados: {_repoEstudiantes.ObtenerTodos().Count}");
            Console.ResetColor();
        }

    }
}