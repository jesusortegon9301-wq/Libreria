# LibraryLab.Infrastructure

## Responsabilidad esperada
Implementar persistencia y detalles técnicos de infraestructura.

## Tecnologías conocidas
EF Core 8; proveedor documentado `Pomelo.EntityFrameworkCore.MySql` 8.0.3; MySQL/MariaDB. El comando conocido sitúa migraciones bajo `Persistence/Migrations`.

## Inventariar
DbContext real, DbSet, configuraciones Fluent API, repositorios, migraciones y versión del servidor.

## Verificación
No guardar credenciales. Revisar migraciones antes de aplicarlas y comprobar compatibilidad con la versión real del motor.
