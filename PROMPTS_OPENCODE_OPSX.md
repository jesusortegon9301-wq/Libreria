# Prompts para OpenCode + OPSX

## 1. Explorar
```text
/opsx:explorar
Analiza LibraryLab sin modificar archivos. Lee AGENTS.md, README.md, openspec/config.yaml,
specs y cambios OpenSpec existentes. Inspecciona proyectos, endpoints, DTO, entidades,
casos de uso, DbContext, EF Core, migraciones y pruebas. Entrega mapa de arquitectura,
rutas verificadas, comandos de build/test y discrepancias documentales. Distingue hechos
de hipótesis. Respeta TODO(STUDENT).
```

## 2. Proponer
```text
/opsx:proponer
Prepara un cambio documental para sincronizar README y specs con el código verificado.
Incluye proposal.md, specs/, design.md y tasks.md en el formato que exige la instalación.
Cubre arquitectura, dominio, API, persistencia, pruebas y componentes. No cambies código
de producción, migraciones ni datos. No inventes contratos. Incluye criterios verificables
y detente para aprobación antes de aplicar.
```

## 3. Aplicar, después de aprobar
```text
/opsx:aplicar
Aplica exclusivamente las tareas aprobadas. Actualiza README, specs y documentos de
componentes con evidencia del repositorio. No alteres producción, contratos, esquema ni
pruebas en el cambio documental. No incluyas secretos. Revisa el diff.
```

## 4. Verificar
```text
/opsx:verificar
Verifica cada criterio de aceptación. Comprueba enlaces, rutas, comandos, endpoints y
contratos. Ejecuta dotnet build y dotnet test si el entorno lo permite y registra los
resultados reales. Comprueba secretos, cambios fuera de alcance y TODO(STUDENT). Indica
qué se verificó y qué no pudo ejecutarse.
```
