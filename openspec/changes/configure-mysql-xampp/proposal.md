# Proposal

## Why

La solución LibraryLab está completa y usa EF Core 8 con `Pomelo.EntityFrameworkCore.MySql`. Sin embargo, la configuración fija la versión de servidor MySQL 8.0.46 (`Database:ServerVersion`) y asume una base `library_lab` con el usuario `library_lab`. En el equipo actual el motor disponible es **MariaDB 10.4.32** (XAMPP) y la base `library_lab` no existe, por lo que la API no puede conectarse ni persistir datos.

Este cambio prepara el motor local y ajusta la configuración para que la API funcione contra el servidor disponible, manteniendo MySQL como único motor admitido y sin versionar credenciales.

## What Changes

- Crear la base `library_lab` (utf8mb4) y el usuario `library_lab` con permisos sobre esa base en el servidor local.
- Ajustar la configuración del proveedor EF Core para detectar automáticamente el motor (MySQL o MariaDB) en lugar de fijar MySQL 8.0.46.
- Configurar la cadena de conexión mediante user-secrets o variable de entorno, sin credenciales en el repositorio.
- Aplicar la migración inicial `20261003001904_InitialCreate` contra la base creada y verificar el esquema.

## Capabilities

### New Capabilities

- `mysql-xampp-persistence`: persistencia de LibraryLab operativa contra el motor local (XAMPP), con esquema creado por migración y conexión configurable.

### Modified Capabilities

Ninguna. Los contratos existentes de dominio, API y persistencia no cambian.

## Impact

El alcance se limita a la configuración de persistencia y a la creación de la base de datos local. No modifica entidades, casos de uso, endpoints, esquema de tablas ni pruebas de producción. Los cinco ejercicios académicos (`TODO(STUDENT)`) permanecen protegidos. No se añaden contenedores ni configuración de orquestación.
