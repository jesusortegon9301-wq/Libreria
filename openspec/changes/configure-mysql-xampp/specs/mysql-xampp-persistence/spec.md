# Spec Delta

## Purpose

Garantizar que LibraryLab persiste en el motor de base de datos local disponible (MySQL/MariaDB en XAMPP), con el esquema creado por migración y la cadena de conexión configurable sin credenciales versionadas.

## ADDED Requirements

### Requirement: La API usa el motor local disponible

Infrastructure MUST configurar EF Core 8 con `Pomelo.EntityFrameworkCore.MySql` y detectar automáticamente si el servidor es MySQL o MariaDB, de modo que la API pueda conectarse al motor local sin fijar una versión incompatible. La cadena de conexión MUST obtenerse de `ConnectionStrings:LibraryLab` o de la variable `ConnectionStrings__LibraryLab`.

#### Scenario: Conectar al motor local

- **WHEN** la API se inicia con una cadena de conexión válida al motor local
- **THEN** Infrastructure se conecta al servidor disponible y persiste los datos en `library_lab`

#### Scenario: No fijar una versión incompatible

- **WHEN** el servidor local es MariaDB y no MySQL 8
- **THEN** la configuración sigue conectando mediante la detección automática del servidor en lugar de fallar por versión

### Requirement: El esquema se crea desde una base vacía

El proyecto MUST permitir aplicar la migración inicial `20261003001904_InitialCreate` contra la base `library_lab`, creando `Users`, `Resources`, `Books`, `Equipment` y `Loans` con claves primarias, claves foráneas, índices y restricciones.

#### Scenario: Aplicar la migración inicial

- **WHEN** se ejecuta `dotnet ef database update` contra una base `library_lab` vacía
- **THEN** se crean las cinco tablas del dominio con claves primarias, claves foráneas, índices y restricciones

### Requirement: La conexión no incluye credenciales versionadas

LibraryLab MUST obtener la cadena de conexión desde configuración o variables de entorno y MUST NOT incluir credenciales reales en el repositorio. El nombre lógico de la base es `library_lab` y el usuario de desarrollo es `library_lab`.

#### Scenario: Configurar la conexión fuera del código

- **WHEN** una persona define `ConnectionStrings:LibraryLab` mediante user-secrets o variable de entorno
- **THEN** la API usa esa conexión sin credenciales incrustadas en archivos fuente

### Requirement: Los datos iniciales son mínimos y repetibles

El inicializador de la API MUST aplicar migraciones pendientes e insertar un usuario, un libro y un equipo de demostración solo si sus tablas están vacías, y MUST ser idempotente.

#### Scenario: Inicializar más de una vez

- **WHEN** se activa la inicialización en dos arranques consecutivos
- **THEN** los datos de demostración no se duplican
