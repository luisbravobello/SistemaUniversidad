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
        // --- 1. Método ConvertirDatos
        
        public static string ConvertirDatos(object dato)
        {
            

            return dato switch
            {
                // Pattern Matching con tipo y variable local (value)
                int value =>
                    $"INT: {value} -> Formateado (N0): {value:N0}",

                double value =>
                    $"DOUBLE: {value} -> Formateado (F2): {value:F2}",

                
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

        // --- 2. Método ParsearCalificacion
        
        public static decimal? ParsearCalificacion(string entrada)
        {
            
            if (decimal.TryParse(entrada,
                                 NumberStyles.Any,
                                 CultureInfo.InvariantCulture, 
                                 out decimal calificacion))
            {
                
                if (calificacion >= 0m && calificacion <= 10m)
                {
                    return calificacion;
                }
            }

            
            return null;
        }

        // Boxing y Unboxing 
        public static void DemostrarBoxingUnboxing(decimal calificacion)
        {
            Console.WriteLine("\n*** Demostración de Tipos Especiales ***");

            
            decimal valorDecimal = calificacion; 
            object boxedCalificacion = valorDecimal;

            Console.WriteLine($"Decimal (Stack): {valorDecimal}");
            Console.WriteLine($"Object (Heap, Boxing): {boxedCalificacion}");

            
            if (boxedCalificacion is decimal)
            {
               
                decimal unboxedCalificacion = (decimal)boxedCalificacion;
                Console.WriteLine($"Decimal (Stack, Unboxing): {unboxedCalificacion}");

               
                unboxedCalificacion = 0m;
                Console.WriteLine($"Valor Boxed NO cambia: {boxedCalificacion}");
            }
        }
    }
}
