# 🏨 Sistema de Reservaciones — Senator
**Complejos Turísticos Senator | República Dominicana**
**Proyecto C# | Aplicación de Consola | Paradigma Procedural**

---

## 🍽️ Restaurantes y Capacidades

| Restaurante | Especialidad | Grupos/Turno |
|-------------|-------------|--------------|
| Ember 🥩 | Carnes | 3 |
| Zao 🍣 | Japonés | 4 |
| Grappa 🍝 | Italiano | 2 |
| Larimar 🦞 | Mariscos | 3 |

**Turnos:** A (6:00–8:00 PM) · B (8:00–10:00 PM)

---

## ✅ Funcionalidades

| # | Función | Descripción |
|---|---------|-------------|
| 1 | 🆕 Realizar reservación | Nombre, personas, restaurante, turno + validaciones |
| 2 | ❌ Eliminar reserva | Busca y elimina con confirmación |
| 3 | 👀 Ver disponibilidad | Cupos por restaurante con barra visual |
| 4 | 🖨️ Imprimir listado | Reporte detallado por restaurante y turno |

---

## 🛠️ Requerimientos técnicos cubiertos

- **Arrays / Listas**: Arreglos 3D `[restaurante, turno, grupo]` para nombres, personas y fechas
- **Funciones void y return**: Cada funcionalidad en su propio método (`RealizarReservacion`, `EliminarReserva`, `VerDisponibilidad`, `ImprimirListado`, etc.)
- **Strings**: `NormalizarNombre()` → `Trim()` + `ToTitleCase()` para limpiar y normalizar
- **Fechas**: `DateTime.Now` registra el timestamp de cada reserva; se muestra en el reporte
- **Lógica de capacidad**: Valida `contadorGrupos[rest, turno] >= CAPACIDAD_MAX[rest, turno]` antes de confirmar

---

## 🚀 Cómo ejecutar

### Requisitos
- .NET SDK 8.0+ → https://dotnet.microsoft.com/download

### Pasos
```bash
cd Senator
dotnet run
```

### Visual Studio
1. Abrir `Senator.csproj`
2. Presionar **F5**

### VS Code
```bash
dotnet run
```

---

## 🏗️ Estructura del código

```
Senator/
├── Senator.csproj       ← Configuración del proyecto
├── Program.cs           ← Todo el código fuente
└── README.md            ← Este archivo
```

### Métodos implementados

| Método | Tipo | Descripción |
|--------|------|-------------|
| `MostrarMenu()` | void | Menú principal con opciones |
| `RealizarReservacion()` | void | Alta de reserva con validaciones |
| `EliminarReserva()` | void | Baja de reserva por nombre |
| `VerDisponibilidad()` | void | Cupos disponibles por turno |
| `ImprimirListado()` | void | Reporte por restaurante y turno |
| `SeleccionarRestaurante()` | int | Sub-menú de restaurantes |
| `SeleccionarTurno()` | int | Sub-menú de turnos |
| `NormalizarNombre(string)` | string | Trim + ToTitleCase |
| `ObtenerNombreCorto(int)` | string | Nombre legible del restaurante |
| `ExisteCliente(...)` | bool | Detecta duplicados |
| `GenerarBarra(int, int)` | string | Barra visual de ocupación |
| `LeerEntero(string)` | int | Entrada segura con validación |
| `Mensaje(string, Color)` | void | Output con color |
| `Separador(string)` | void | Divisor visual de secciones |
| `InicializarArreglos()` | void | Rellena strings vacíos |
