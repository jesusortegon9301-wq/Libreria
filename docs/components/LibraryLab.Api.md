# LibraryLab.Api

## Responsabilidad
Capa de entrada HTTP: configuración de la aplicación, endpoints/controladores y publicación OpenAPI/Swagger, según la implementación real.

## Revisar
`Program.cs`, controladores/Minimal APIs, DTO, validación, middleware, Swagger y `Properties/launchSettings.json`.

## Datos conocidos
Proyecto de inicio de EF Core: `src/LibraryLab.Api`. Clave de conexión conocida: `ConnectionStrings:LibraryLab`. URL previamente mencionada: `https://localhost:7111/swagger/index.html`; confirmar en el perfil activo.

## Verificación
Las rutas, DTO, códigos HTTP y errores documentados deben coincidir con código, Swagger y pruebas. No guardar secretos ni duplicar reglas complejas de dominio en controladores.
