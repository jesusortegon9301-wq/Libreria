# Design

## Context

LibraryLab es una solución .NET 8 con Clean Architecture (Domain, Application, Infrastructure, Api) y persistencia exclusiva en MySQL mediante EF Core 8 y `Pomelo.EntityFrameworkCore.MySql` 8.0.3. La configuración actual (`src/LibraryLab.Infrastructure/DependencyInjection.cs`) obtiene la cadena de conexión de `ConnectionStrings:LibraryLab` o de la variable `ConnectionStrings__LibraryLab` y fija la versión del servidor con `new MySqlServerVersion(serverVersion)`, donde `serverVersion` proviene de `Database:ServerVersion` (valor predeterminado `8.0.46`).

En el equipo actual, XAMPP expone **MariaDB 10.4.32** en el puerto 3306 y la base `library_lab` no existe. Fijar MySQL 8.0.46 contra MariaDB puede producir SQL incompatible y la ausencia de la base impide cualquier conexión.

## Goals / Non-Goals

**Goals:**
- Que la API conecte y persista en el motor local disponible (MariaDB 10.4.32 en XAMPP).
- Mantener MySQL/MariaDB como único motor admitido, sin añadir SQL Server, PostgreSQL, SQLite ni InMemory.
- Mantener la cadena de conexión configurable y sin credenciales versionadas.
- Mantener la migración inicial como mecanismo oficial de creación del esquema.
- Mantener los cinco ejercicios académicos sin implementar.

**Non-Goals:**
- Cambiar el dominio, los casos de uso, los endpoints ni los DTO.
- Modificar el esquema de tablas ni las restricciones ya definidas.
- Implementar los ejercicios `TODO(STUDENT)` ni debilitar sus pruebas.
- Añadir Docker, Compose ni configuración de contenedores.

## Decisions

### Detección automática del motor

Sustituir `new MySqlServerVersion(serverVersion)` por `ServerVersion.AutoDetect(connectionString)` en `src/LibraryLab.Infrastructure/DependencyInjection.cs`. De este modo Pomelo selecciona `MySqlServerVersion` o `MariaDbServerVersion` según el servidor real, y la API funciona tanto con MySQL 8 como con MariaDB 10.4 sin fijar una versión incompatible. `Database:ServerVersion` se conserva como información opcional, no como requisito.

### Base de datos y usuario de desarrollo

Crear la base `library_lab` con `CHARACTER SET utf8mb4` y el usuario `library_lab`@`localhost` con contraseña `library_lab` y permisos sobre `library_lab.*`, tal como documenta el README del proyecto.

### Cadena de conexión externa

La conexión se obtiene de `ConnectionStrings:LibraryLab` (user-secrets) o de la variable `ConnectionStrings__LibraryLab`. No se escriben credenciales en archivos versionados. La contraseña de desarrollo solo existe en el perfil del usuario o en el entorno local.

### Esquema creado por migración

El esquema se crea aplicando `20261003001904_InitialCreate` con `dotnet ef database update`. `specs/mysql-initial.sql` permanece como referencia del DDL generado, no como mecanismo principal.

### Inicialización idempotente

El inicializador de la API aplica migraciones pendientes e inserta un usuario, un libro y un equipo de demostración solo si sus tablas están vacías, y solo cuando `Database__InitializeOnStartup=true`.

## Risks / Trade-offs

- **[MariaDB y MySQL 8 no son idénticos] →** Usar `ServerVersion.AutoDetect` evita fijar DDL de MySQL 8 contra MariaDB; verificar la migración en el motor real antes de dar por concluido el cambio.
- **[La contraseña de desarrollo viaja en texto plano en local] →** Aceptable únicamente en el entorno local; no se versiona y se documenta la ubicación externa.
- **[El SDK instalado es 10 y el destino es net8.0] →** Verificar que `dotnet build` compile la solución completa antes de aplicar la migración; el proyecto admite SDK .NET 8 o posterior.
- **[La documentación existente referencia una raíz antigua] →** El cambio usa rutas relativas desde la raíz de la solución para no depender de la ubicación física del equipo.

## Migration Plan

1. Crear la base `library_lab` y el usuario `library_lab` en el servidor local.
2. Ajustar `DependencyInjection.cs` para detectar el motor automáticamente.
3. Configurar la cadena de conexión de forma externa.
4. Restaurar herramientas y aplicar `dotnet ef database update`.
5. Verificar tablas, claves, índices y restricciones; arrancar la API y confirmar la conexión.

**Vuelta atrás:** revertir el cambio de `DependencyInjection.cs` y, si se desea, eliminar la base con `DROP DATABASE library_lab`. No se borran datos de otras bases.
