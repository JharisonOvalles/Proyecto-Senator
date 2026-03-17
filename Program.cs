// ============================================================
//   Sistema de Reservaciones — Complejos Turísticos Senator
//   Restaurantes Privados | Aplicación de Consola C#
//   Paradigma procedural: arrays, métodos, strings, fechas
// ============================================================

using System;

class Program
{
    // ─────────────────────────────────────────────────────────
    //  CONSTANTES DEL SISTEMA
    // ─────────────────────────────────────────────────────────

    // Índices de restaurantes
    const int EMBER   = 0;
    const int ZAO     = 1;
    const int GRAPPA  = 2;
    const int LARIMAR = 3;
    const int TOTAL_RESTAURANTES = 4;

    // Índices de turnos
    const int TURNO_A = 0;   // 6:00 PM – 8:00 PM
    const int TURNO_B = 1;   // 8:00 PM – 10:00 PM
    const int TOTAL_TURNOS = 2;

    // Capacidad máxima de grupos por restaurante por turno
    // [restaurante, turno]
    static readonly int[,] CAPACIDAD_MAX = {
        // Turno A  Turno B
        {  3,       3  },   // Ember   🥩
        {  4,       4  },   // Zao     🍣
        {  2,       2  },   // Grappa  🍝
        {  3,       3  },   // Larimar 🦞
    };

    // Nombres de restaurantes y su especialidad
    static readonly string[] NOMBRE_REST = {
        "Ember   🥩 Carnes",
        "Zao     🍣 Japonés",
        "Grappa  🍝 Italiano",
        "Larimar 🦞 Mariscos"
    };

    static readonly string[] NOMBRE_TURNO = {
        "Turno A — 6:00 PM a 8:00 PM",
        "Turno B — 8:00 PM a 10:00 PM"
    };

    // ─────────────────────────────────────────────────────────
    //  DATOS DE RESERVACIONES
    //  Se usan arreglos paralelos por [restaurante, turno]
    //  Cada celda almacena hasta CAPACIDAD_MAX grupos
    // ─────────────────────────────────────────────────────────
    const int MAX_GRUPOS = 4;   // máximo posible en cualquier restaurante

    // nombre del cliente
    static string[,,] resNombre   = new string[TOTAL_RESTAURANTES, TOTAL_TURNOS, MAX_GRUPOS];
    // cantidad de personas en el grupo
    static int[,,]    resPersonas = new int   [TOTAL_RESTAURANTES, TOTAL_TURNOS, MAX_GRUPOS];
    // fecha/hora en que se realizó la reserva (para el reporte)
    static DateTime[,,] resFecha  = new DateTime[TOTAL_RESTAURANTES, TOTAL_TURNOS, MAX_GRUPOS];
    // cuántos grupos hay actualmente por [restaurante, turno]
    static int[,] contadorGrupos  = new int[TOTAL_RESTAURANTES, TOTAL_TURNOS];

    // ─────────────────────────────────────────────────────────
    //  PUNTO DE ENTRADA
    // ─────────────────────────────────────────────────────────
    static void Main(string[] args)
    {
        InicializarArreglos();

        int opcion = -1;
        while (opcion != 0)
        {
            MostrarMenu();
            opcion = LeerEntero("  Elige una opción: ");

            switch (opcion)
            {
                case 1: RealizarReservacion();  break;
                case 2: EliminarReserva();      break;
                case 3: VerDisponibilidad();    break;
                case 4: ImprimirListado();      break;
                case 0:
                    Mensaje("\n  ¡Hasta pronto! Que disfruten la cena. 🌴", ConsoleColor.Cyan);
                    break;
                default:
                    Mensaje("  Opción no válida. Intenta de nuevo.", ConsoleColor.Red);
                    break;
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    //  MENÚ PRINCIPAL
    // ─────────────────────────────────────────────────────────
    static void MostrarMenu()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  ╔══════════════════════════════════════════════╗");
        Console.WriteLine("  ║   🏨  SENATOR — SISTEMA DE RESERVACIONES    ║");
        Console.WriteLine("  ║      Restaurantes Privados • RD              ║");
        Console.WriteLine("  ╠══════════════════════════════════════════════╣");
        Console.WriteLine("  ║  1.  🆕  Realizar reservación               ║");
        Console.WriteLine("  ║  2.  ❌  Eliminar reserva                   ║");
        Console.WriteLine("  ║  3.  👀  Ver disponibilidad                 ║");
        Console.WriteLine("  ║  4.  🖨️  Imprimir listado                   ║");
        Console.WriteLine("  ║  0.  🚪  Salir                              ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    // ─────────────────────────────────────────────────────────
    //  1. REALIZAR RESERVACIÓN
    // ─────────────────────────────────────────────────────────
    static void RealizarReservacion()
    {
        Separador("NUEVA RESERVACIÓN");

        // — Nombre del cliente (trim + ToTitleCase)
        Console.Write("  Nombre del cliente: ");
        string nombre = NormalizarNombre(Console.ReadLine()!);
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Mensaje("  El nombre no puede estar vacío.", ConsoleColor.Red);
            return;
        }

        // — Cantidad de personas
        int personas = LeerEntero("  Cantidad de personas en el grupo: ");
        if (personas <= 0)
        {
            Mensaje("  La cantidad debe ser mayor a 0.", ConsoleColor.Red);
            return;
        }

        // — Selección de restaurante
        int rest = SeleccionarRestaurante();
        if (rest == -1) return;

        // — Selección de turno
        int turno = SeleccionarTurno();
        if (turno == -1) return;

        // — Verificar disponibilidad
        int gruposActuales = contadorGrupos[rest, turno];
        int capacidad      = CAPACIDAD_MAX[rest, turno];

        if (gruposActuales >= capacidad)
        {
            Mensaje($"\n  ⛔  FULL — {ObtenerNombreCorto(rest)} no tiene cupos para el {NOMBRE_TURNO[turno]}.", ConsoleColor.Red);
            Mensaje($"  Capacidad máxima ({capacidad} grupos) alcanzada.", ConsoleColor.Red);
            return;
        }

        // — Verificar nombre duplicado en ese slot
        if (ExisteCliente(nombre, rest, turno))
        {
            Mensaje($"  Ya existe una reserva para '{nombre}' en ese restaurante y turno.", ConsoleColor.Red);
            return;
        }

        // — Guardar reserva
        int slot = gruposActuales;
        resNombre  [rest, turno, slot] = nombre;
        resPersonas[rest, turno, slot] = personas;
        resFecha   [rest, turno, slot] = DateTime.Now;
        contadorGrupos[rest, turno]++;

        Mensaje($"\n  ✅  Reserva confirmada:", ConsoleColor.Green);
        Console.WriteLine($"      Cliente   : {nombre}");
        Console.WriteLine($"      Personas  : {personas}");
        Console.WriteLine($"      Restaurante: {ObtenerNombreCorto(rest)}");
        Console.WriteLine($"      Turno     : {NOMBRE_TURNO[turno]}");
        Console.WriteLine($"      Registrada: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine($"      Cupos restantes: {capacidad - contadorGrupos[rest, turno]} de {capacidad}");
    }

    // ─────────────────────────────────────────────────────────
    //  2. ELIMINAR RESERVA
    // ─────────────────────────────────────────────────────────
    static void EliminarReserva()
    {
        Separador("ELIMINAR RESERVA");

        Console.Write("  Nombre del cliente a eliminar: ");
        string nombre = NormalizarNombre(Console.ReadLine()!);

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Mensaje("  Nombre inválido.", ConsoleColor.Red);
            return;
        }

        // Buscar en todos los restaurantes y turnos
        bool encontrado = false;

        for (int r = 0; r < TOTAL_RESTAURANTES && !encontrado; r++)
        {
            for (int t = 0; t < TOTAL_TURNOS && !encontrado; t++)
            {
                int total = contadorGrupos[r, t];
                for (int i = 0; i < total; i++)
                {
                    if (resNombre[r, t, i].ToUpper() == nombre.ToUpper())
                    {
                        // Mostrar los datos antes de borrar
                        Console.WriteLine();
                        Console.WriteLine($"  Reserva encontrada:");
                        Console.WriteLine($"    Cliente    : {resNombre[r, t, i]}");
                        Console.WriteLine($"    Personas   : {resPersonas[r, t, i]}");
                        Console.WriteLine($"    Restaurante: {ObtenerNombreCorto(r)}");
                        Console.WriteLine($"    Turno      : {NOMBRE_TURNO[t]}");

                        Console.Write("\n  ¿Confirmar eliminación? (s/n): ");
                        string conf = Console.ReadLine()!.Trim().ToLower();
                        if (conf != "s")
                        {
                            Mensaje("  Operación cancelada.", ConsoleColor.DarkYellow);
                            return;
                        }

                        // Desplazar elementos para cerrar el hueco
                        for (int k = i; k < total - 1; k++)
                        {
                            resNombre  [r, t, k] = resNombre  [r, t, k + 1];
                            resPersonas[r, t, k] = resPersonas[r, t, k + 1];
                            resFecha   [r, t, k] = resFecha   [r, t, k + 1];
                        }

                        // Limpiar el último slot liberado
                        resNombre  [r, t, total - 1] = string.Empty;
                        resPersonas[r, t, total - 1] = 0;
                        resFecha   [r, t, total - 1] = default;
                        contadorGrupos[r, t]--;

                        Mensaje($"\n  ✅  Reserva de '{nombre}' eliminada correctamente.", ConsoleColor.Green);
                        encontrado = true;
                    }
                }
            }
        }

        if (!encontrado)
            Mensaje($"\n  ⚠️   No se encontró ninguna reserva para '{nombre}'.", ConsoleColor.DarkYellow);
    }

    // ─────────────────────────────────────────────────────────
    //  3. VER DISPONIBILIDAD
    // ─────────────────────────────────────────────────────────
    static void VerDisponibilidad()
    {
        Separador("DISPONIBILIDAD");

        int turno = SeleccionarTurno();
        if (turno == -1) return;

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  Disponibilidad para: {NOMBRE_TURNO[turno]}");
        Console.WriteLine($"  Consulta realizada : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine();
        Console.WriteLine($"  {"Restaurante",-28} {"Ocupado",8} {"Disponible",11} {"Capacidad",10}  Estado");
        Console.WriteLine("  " + new string('─', 70));
        Console.ResetColor();

        for (int r = 0; r < TOTAL_RESTAURANTES; r++)
        {
            int ocupados    = contadorGrupos[r, turno];
            int capacidad   = CAPACIDAD_MAX[r, turno];
            int disponibles = capacidad - ocupados;

            // Barra visual de ocupación
            string barra = GenerarBarra(ocupados, capacidad);

            // Color según disponibilidad
            if (disponibles == 0)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (disponibles == 1)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            string estado = disponibles == 0 ? "⛔ FULL"
                          : disponibles == 1 ? "⚠️  1 cupo"
                          : $"✅ {disponibles} cupos";

            Console.WriteLine($"  {NOMBRE_REST[r],-28} {ocupados,8} {disponibles,11} {capacidad,10}  {estado}  {barra}");
            Console.ResetColor();
        }
    }

    // ─────────────────────────────────────────────────────────
    //  4. IMPRIMIR LISTADO
    // ─────────────────────────────────────────────────────────
    static void ImprimirListado()
    {
        Separador("IMPRIMIR LISTADO");

        int rest = SeleccionarRestaurante();
        if (rest == -1) return;

        int turno = SeleccionarTurno();
        if (turno == -1) return;

        int total = contadorGrupos[rest, turno];

        // Encabezado del reporte
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine($"  ║  REPORTE DE RESERVACIONES — SENATOR                     ║");
        Console.WriteLine($"  ║  Restaurante : {ObtenerNombreCorto(rest),-43}║");
        Console.WriteLine($"  ║  Turno       : {NOMBRE_TURNO[turno],-43}║");
        Console.WriteLine($"  ║  Generado    : {DateTime.Now:dd/MM/yyyy HH:mm:ss}          ║");
        Console.WriteLine($"  ║  Grupos      : {total} / {CAPACIDAD_MAX[rest, turno],-42}║");
        Console.WriteLine("  ╠══════════════════════════════════════════════════════════╣");
        Console.ResetColor();

        if (total == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  ║  (Sin reservaciones en este turno)                      ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            return;
        }

        // Calcular total de personas
        int totalPersonas = 0;

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  ║  {"#",-4} {"Cliente",-25} {"Personas",9} {"Reservado el",-22}║");
        Console.WriteLine("  ╠══════════════════════════════════════════════════════════╣");

        for (int i = 0; i < total; i++)
        {
            totalPersonas += resPersonas[rest, turno, i];
            string fecha = resFecha[rest, turno, i].ToString("dd/MM/yyyy HH:mm");
            Console.WriteLine($"  ║  {i + 1,-4} {resNombre[rest, turno, i],-25} {resPersonas[rest, turno, i],9} {fecha,-22}║");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╠══════════════════════════════════════════════════════════╣");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ║  {"TOTAL PERSONAS:",-30} {totalPersonas,9}                      ║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    // ─────────────────────────────────────────────────────────
    //  FUNCIONES AUXILIARES — SELECCIÓN
    // ─────────────────────────────────────────────────────────

    /// <summary>Muestra el menú de restaurantes y retorna el índice elegido (-1 = cancelar).</summary>
    static int SeleccionarRestaurante()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  Restaurantes disponibles:");
        for (int r = 0; r < TOTAL_RESTAURANTES; r++)
            Console.WriteLine($"    {r + 1}. {NOMBRE_REST[r]}");
        Console.ResetColor();

        int idx = LeerEntero("  Selecciona el restaurante (número): ") - 1;

        if (idx < 0 || idx >= TOTAL_RESTAURANTES)
        {
            Mensaje("  Restaurante inválido.", ConsoleColor.Red);
            return -1;
        }
        return idx;
    }

    /// <summary>Muestra el menú de turnos y retorna el índice elegido (-1 = cancelar).</summary>
    static int SeleccionarTurno()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  Turnos disponibles:");
        for (int t = 0; t < TOTAL_TURNOS; t++)
            Console.WriteLine($"    {t + 1}. {NOMBRE_TURNO[t]}");
        Console.ResetColor();

        int idx = LeerEntero("  Selecciona el turno (número): ") - 1;

        if (idx < 0 || idx >= TOTAL_TURNOS)
        {
            Mensaje("  Turno inválido.", ConsoleColor.Red);
            return -1;
        }
        return idx;
    }

    // ─────────────────────────────────────────────────────────
    //  FUNCIONES AUXILIARES — STRINGS Y FECHAS
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Limpia el nombre: quita espacios extremos y normaliza a Title Case.
    /// Requisito: "Fechas y Strings — limpiar nombres, normalizar mayúsculas".
    /// </summary>
    static string NormalizarNombre(string entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada)) return string.Empty;

        // 1. Quitar espacios al inicio/final
        string limpio = entrada.Trim();

        // 2. Convertir a Title Case (primera letra de cada palabra en mayúscula)
        System.Globalization.TextInfo ti =
            System.Globalization.CultureInfo.CurrentCulture.TextInfo;

        return ti.ToTitleCase(limpio.ToLower());
    }

    /// <summary>Retorna el nombre corto del restaurante (sin emoji para comparaciones).</summary>
    static string ObtenerNombreCorto(int rest)
    {
        return rest switch {
            EMBER   => "Ember (Carnes)",
            ZAO     => "Zao (Japonés)",
            GRAPPA  => "Grappa (Italiano)",
            LARIMAR => "Larimar (Mariscos)",
            _       => "Desconocido"
        };
    }

    /// <summary>Verifica si ya existe un cliente en el mismo restaurante y turno.</summary>
    static bool ExisteCliente(string nombre, int rest, int turno)
    {
        int total = contadorGrupos[rest, turno];
        for (int i = 0; i < total; i++)
            if (resNombre[rest, turno, i].ToUpper() == nombre.ToUpper())
                return true;
        return false;
    }

    /// <summary>Genera una barra visual de ocupación. Ej: [███░░]</summary>
    static string GenerarBarra(int ocupados, int capacidad)
    {
        string barra = "[";
        for (int i = 0; i < capacidad; i++)
            barra += i < ocupados ? "█" : "░";
        barra += "]";
        return barra;
    }

    // ─────────────────────────────────────────────────────────
    //  FUNCIONES AUXILIARES — ENTRADA / SALIDA
    // ─────────────────────────────────────────────────────────

    /// <summary>Lee un entero con validación de formato.</summary>
    static int LeerEntero(string prompt)
    {
        int valor;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out valor)) return valor;
            Mensaje("  Entrada inválida. Ingresa un número entero.", ConsoleColor.Red);
        }
    }

    /// <summary>Muestra un mensaje en el color indicado y restaura el color.</summary>
    static void Mensaje(string texto, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(texto);
        Console.ResetColor();
    }

    /// <summary>Imprime un separador de sección con título.</summary>
    static void Separador(string titulo)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  ── {titulo} ".PadRight(52, '─'));
        Console.ResetColor();
    }

    // ─────────────────────────────────────────────────────────
    //  INICIALIZACIÓN
    // ─────────────────────────────────────────────────────────

    /// <summary>Rellena todos los nombres con string.Empty para evitar nulls.</summary>
    static void InicializarArreglos()
    {
        for (int r = 0; r < TOTAL_RESTAURANTES; r++)
            for (int t = 0; t < TOTAL_TURNOS; t++)
                for (int i = 0; i < MAX_GRUPOS; i++)
                    resNombre[r, t, i] = string.Empty;
    }
}
