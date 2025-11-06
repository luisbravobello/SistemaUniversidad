using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaUniversidad.Sistema
{
    public static class TiposEspeciales
    {
        // --- 1. Método ConvertirDatos(object dato) ---
        // Recibe un 'object' (potencial BOXING) y usa Pattern Matching para identificar el tipo.
        public static string ConvertirDatos(object dato)
        {
            // El 'object dato' es donde ocurre el Boxing si se pasa un tipo de valor.

            return dato switch
            {
                // Pattern Matching con tipo y variable local (value)
                int value =>
                    $"INT: {value} -> Formateado (N0): {value:N0}",

                double value =>
                    $"DOUBLE: {value} -> Formateado (F2): {value:F2}",

                // Unboxing implícito: el compilador sabe que 'value' es un string
                string value =>
                    $"STRING: '{value}' -> Longitud: {value.Length}",

                DateTime value =>
                    $"DATETIME: {value:yyyy-MM-dd} -> Hora: {value:HH:mm:ss}",

                null =>
                    "NULL: El dato es nulo.",

                // Patrón de descarte para cualquier otro tipo
                _ =>
                    $"TIPO NO SOPORTADO: {dato.GetType().Name}. Valor: {dato.ToString()}"
            };
        }

        // --- 2. Método ParsearCalificacion(string entrada) ---
        // Usa TryParse para una conversión segura, incluyendo validación de rango.
        public static decimal? ParsearCalificacion(string entrada)
        {
            // Usamos decimal.TryParse para evitar excepciones
            if (decimal.TryParse(entrada,
                                 NumberStyles.Any,
                                 CultureInfo.InvariantCulture, // Usamos punto decimal (ej: 7.5)
                                 out decimal calificacion))
            {
                // Validación de rango (0 a 10)
                if (calificacion >= 0m && calificacion <= 10m)
                {
                    return calificacion;
                }
            }

            // Retorna null (decimal?) si la conversión falla o está fuera de rango
            return null;
        }

        // --- 3. Demostración de Boxing y Unboxing ---
        public static void DemostrarBoxingUnboxing(decimal calificacion)
        {
            Console.WriteLine("\n*** Demostración de Tipos Especiales ***");

            // 1. BOXING: El tipo de valor (decimal) se envuelve en el tipo de referencia (object)
            decimal valorDecimal = calificacion; // Vive en el Stack
            object boxedCalificacion = valorDecimal; // El valor es copiado al Heap

            Console.WriteLine($"Decimal (Stack): {valorDecimal}");
            Console.WriteLine($"Object (Heap, Boxing): {boxedCalificacion}");

            // 2. UNBOXING: El tipo de referencia es convertido de vuelta al tipo de valor
            if (boxedCalificacion is decimal)
            {
                // Cast explícito requerido. El valor se copia del Heap al Stack.
                decimal unboxedCalificacion = (decimal)boxedCalificacion;
                Console.WriteLine($"Decimal (Stack, Unboxing): {unboxedCalificacion}");

                // Demostrar que son copias independientes
                unboxedCalificacion = 0m;
                Console.WriteLine($"Valor Boxed NO cambia: {boxedCalificacion}");
            }
        }
    }
}
