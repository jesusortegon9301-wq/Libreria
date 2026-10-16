# Tasks

## 1. Preparar el motor local

- [x] 1.1 Crear la base `library_lab` con `CHARACTER SET utf8mb4` en el servidor local.
- [x] 1.2 Crear el usuario `library_lab`@`localhost` con contraseña y otorgar permisos sobre `library_lab.*`.
- [x] 1.3 Verificar la conexión con el nuevo usuario mediante el cliente MySQL.

## 2. Ajustar la configuración del proveedor

- [x] 2.1 Sustituir `new MySqlServerVersion(serverVersion)` por `ServerVersion.AutoDetect(connectionString)` en `src/LibraryLab.Infrastructure/DependencyInjection.cs`.
- [x] 2.2 Compilar la solución completa y confirmar que no hay errores.

## 3. Configurar la cadena de conexión

- [x] 3.1 Definir `ConnectionStrings:LibraryLab` mediante user-secrets o variable de entorno.
- [x] 3.2 Confirmar que ninguna credencial real queda en archivos versionados.

## 4. Aplicar la migración inicial

- [x] 4.1 Restaurar las herramientas locales con `dotnet tool restore`.
- [x] 4.2 Aplicar `dotnet ef database update` contra `library_lab`.
- [x] 4.3 Verificar que se crearon `Users`, `Resources`, `Books`, `Equipment` y `Loans` con claves, índices y restricciones.

## 5. Verificación

- [x] 5.1 Ejecutar la API y confirmar que conecta al motor local.
- [x] 5.2 Ejecutar las pruebas normales y/o el script de verificación contra MySQL real.
- [x] 5.3 Registrar los resultados reales y las tareas pendientes.

## Evidencia de ejecución

Las 13 tareas están completadas contra el servidor local **MariaDB 10.4.32** (XAMPP).

| Comprobación realizada | Resultado |
|---|---|
| Creación de base y usuario | Base `library_lab` (utf8mb4) y usuario `library_lab`@`localhost` con `ALL PRIVILEGES` sobre `library_lab.*`. Verificado con `CREATE TABLE`/`DROP TABLE` de prueba. |
| Reparación de tabla del sistema | Durante la creación, MariaDB reportó corrupción en `mysql.db` (motor Aria). Se ejecutó `REPAIR TABLE mysql.db` y se restauraron los permisos de `pma` sobre `phpmyadmin` (phpMyAdmin). |
| Ajuste del proveedor | `src/LibraryLab.Infrastructure/DependencyInjection.cs` ahora usa `ServerVersion.AutoDetect(connectionString)` con respaldo `MySqlServerVersion(8.0.0)` si el servidor no está disponible. |
| Configuración de appsettings | Se eliminó `"ServerVersion": "8.0.46"` de `src/LibraryLab.Api/appsettings.json` para no forzar MySQL 8 sobre MariaDB. |
| Cadena de conexión | `ConnectionStrings:LibraryLab` guardada con `dotnet user-secrets` (fuera del repositorio). Sin credenciales versionadas. |
| Herramientas | `dotnet tool restore` restauró `dotnet-ef` 8.0.13. |
| Migración | `dotnet ef database update` aplicó `20261003001904_InitialCreate`; se crearon `users`, `resources`, `books`, `equipment`, `loans` y `__EFMigrationsHistory`, con claves primarias, FK compuesta `Loans(ResourceId, ResourceKind)` → `resources(Id, Kind)`, índices y restricciones. |
| Arranque de la API | `dotnet run --project src/LibraryLab.Api` conectó a MariaDB, ejecutó el inicializador e insertó 1 usuario, 1 libro y 1 equipo de demostración. Escuchó en `http://localhost:5000`. |
| Verificación de integración | `scripts/Verify-LibraryLab.ps1 -ApiBaseUrl http://localhost:5000` → **VERIFICACIÓN CORRECTA: 76 aserciones y 38 respuestas HTTP reales** (altas, consultas, préstamos, devoluciones, concurrencia y duplicados). |
| Suite de pruebas | `dotnet test LibraryLab.sln` → **39/39 aprobadas** (Domain 7, Application 5, Patterns 5, Infrastructure 2, Architecture 2, Api 18). |

La API se detuvo después de la verificación. La base `library_lab` conserva el esquema y los datos de demostración.
