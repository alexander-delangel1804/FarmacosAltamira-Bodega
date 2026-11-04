# Matriz de Permisos - Módulo de Bodega Fármacos Altamira

## 1. Descripción General

La matriz de permisos define qué usuarios pueden acceder a cada módulo y realizar qué acciones dentro del sistema. Los permisos se verifican tanto en el cliente (para ocultar/mostrar opciones de menú) como en el servidor (para validar operaciones).

## 2. Roles de Usuario

| Rol | Descripción |
|-----|-------------|
| **Administrador** | Acceso completo a todas las funcionalidades del sistema |
| **Bodegador** | Acceso a operaciones de bodega (existencias, recepciones, ajustes, traslados) |
| **Supervisor** | Acceso a consulta y reportes (dashboard, kárdex, alertas) |
| **Auditor** | Acceso de solo lectura a bitácora y reportes |

---

## 3. Matriz de Permisos por Módulo

| Módulo | Administrador | Bodegador | Supervisor | Auditor | Descripción |
|--------|--------------|-----------|------------|---------|
| Dashboard | ✓ | ✓ | ✓ | ✓ |
| Existencias por Ubicación | ✓ | ✓ | ✓ | ✓ |
| Catálogo de Productos | ✓ | ✓ | ✓ | ✓ |
| Lotes y Caducidades | ✓ | ✓ | ✓ | ✓ |
| Kárdex y Movimientos | ✓ | ✓ | ✓ | ✓ |
| Ajustes de Inventario | ✓ | ✓ | ✓ | ❌ |
| Recepción de Mercancía | ✓ | ✓ | ✓ | ❌ |
| Traslados | ✓ | ✓ | ✓ | ✓ |
| Alertas de Caducidad | ✓ | ✓ | ✓ | ✓ |
| Alertas de Stock Mínimo | ✓ | ✓ | ✓ | ✓ |
| Control FEFO | ✓ | ✓ | ✓ | ✓ |
| Bitácora de Operaciones | ✓ | ❌ | ✓ | ✓ |
| Usuarios y Permisos | ✓ | ❌ | ❌ | ❌ |

**Leyenda:** ✓ = Acceso permitido | ❌ = Sin acceso

---

## 4. Matriz de Permisos por Acción

### 3.1 Catálogo de Productos

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver catálogo | ✓ | ✓ | ✓ | ✓ |
| Buscar productos | ✓ | ✓ | ✓ | ✓ |
| Crear producto | ✓ | ✓ | ❌ | ❌ |
| Editar producto | ✓ | ✓ | ❌ | ❌ |
| Desactivar producto | ✓ | ✓ | ❌ | ❌ |

### 3.2 Existencias por Ubicación

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Consultar existencias | ✓ | ✓ | ✓ | ✓ |
| Filtrar por producto/lote/ubicación | ✓ | ✓ | ✓ | ✓ |
| Paginación | ✓ | ✓ | ✓ | ✓ |

### 3.3 Lotes y Caducidades (Información de Lote)

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver información de lote | ✓ | ✓ | ✓ | ✓ |
| Ver existencias de lote | ✓ | ✓ | ✓ | ✓ |

### 3.4 Kárdex y Movimientos

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Consultar movimientos | ✓ | ✓ | ✓ | ✓ |
| Filtrar por producto/lote/ubicación | ✓ | ✓ | ✓ | ✓ |
| Filtrar por fecha y tipo | ✓ | ✓ | ✓ | ✓ |

### 3.5 Ajustes de Inventario

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver existencias de lote | ✓ | ✓ | ✓ | ✓ |
| Registrar ajuste positivo | ✓ | ✓ | ✓ | ❌ |
| Registrar ajuste negativo | ✓ | ✓ | ✓ | ❌ |

### 3.6 Recepción de Mercancía

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Seleccionar ubicación de destino | ✓ | ✓ | ✓ | ❌ |
| Agregar partidas a recepción | ✓ | ✓ | ✓ | ❌ |
| Registrar recepción | ✓ | ✓ | ✓ | ❌ |

### 3.7 Traslados

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Solicitar traslado | ✓ | ✓ | ✓ | ❌ |
| Despachar traslado | ✓ | ✓ | ✓ | ❌ |
| Recibir traslado | ✓ | ✓ | ✓ | ❌ |
| Consultar historial de traslados | ✓ | ✓ | ✓ | ✓ |

### 3.8 Alertas de Caducidad

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver alertas de caducidad | ✓ | ✓ | ✓ | ✓ |
| Marcar alerta como atendida | ✓ | ✓ | ✓ | ❌ |

### 3.9 Alertas de Stock Mínimo

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver alertas de stock | ✓ | ✓ | ✓ | ✓ |
| Marcar alerta como atendida | ✓ | ✓ | ✓ | ❌ |

### 3.10 Control FEFO

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver lotes elegibles | ✓ | ✓ | ✓ | ✓ |
| Filtrar por estado de lote | ✓ | ✓ | ✓ | ✓ |

### 3.11 Bitácora de Operaciones

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver bitácora | ✓ | ❌ | ✓ | ✓ |
| Filtrar bitácora | ✓ | ❌ | ✓ | ✓ |

### 3.12 Usuarios y Permisos

| Acción | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Ver usuarios | ✓ | ❌ | ❌ | ❌ |
| Crear usuario | ✓ | ❌ | ❌ | ❌ |
| Editar usuario | ✓ | ❌ | ❌ | ❌ |
| Desactivar usuario | ✓ | ❌ | ❌ | ❌ |

---

## 4. Matriz de Permisos por Acción Específica

### Acciones de Escritura (Modifican datos)

| Acción | Requiere Permiso | Módulo |
|--------|-----------------|--------|
| Crear producto | INVENTARIO.CREAR | Catálogo |
| Editar producto | INVENTARIO.EDITAR | Catálogo |
| Desactivar producto | INVENTARIO.DESACTIVAR | Catálogo |
| Registrar recepción | INVENTARIO.RECEPCIONAR | Recepción |
| Registrar ajuste | INVENTARIO.AJUSTAR | Ajustes |
| Solicitar traslado | INVENTARIO.TRASLADAR | Traslados |
| Despachar traslado | INVENTARIO.DESPACHAR | Traslados |
| Recibir traslado | INVENTARIO.RECIBIR | Traslados |
| Crear usuario | USUARIO.CREAR | Usuarios |
| Editar usuario | USUARIO.EDITAR | Usuarios |
| Desactivar usuario | USUARIO.DESACTIVAR | Usuarios |

### Acciones de Lectura (Solo lectura)

| Acción | Requiere Permiso | Módulo |
|--------|-----------------|--------|
| Ver catálogo | INVENTARIO.VER | Catálogo |
| Ver existencias | INVENTARIO.VER | Existencias |
| Ver lotes | INVENTARIO.VER | Lotes |
| Ver kardex | INVENTARIO.VER | Kárdex |
| Ver ajustes | INVENTARIO.VER | Ajustes |
| Ver recepciones | INVENTARIO.VER | Recepción |
| Ver traslados | INVENTARIO.VER | Traslados |
| Ver alertas | ALERTA.VER | Alertas |
| Ver bitácora | BITACORA.VER | Bitácora |
| Ver usuarios | USUARIO.VER | Usuarios |

---

## 7. Implementación de Permisos en el Código

### Verificación de Permisos en ViewModel

```csharp
// Ejemplo: Verificar permiso para ajustes
private bool _puedeAccederAjustes;

public ShellViewModel(ISesionAppService sesionService)
{
    _sesionService = sesionService;
    PuedeAccederAdministracion = _sesionService.PuedeAccederAMinistro("ADMINISTRACION", "USUARIOS");
}
```

### Validación en Servicios de Aplicación

```csharp
public async Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct)
{
    // Validar permisos en el servicio de aplicación
    if (!_sesionService.PuedeAccederAMinistro("INVENTARIO", "RECEPCIONAR"))
    {
        throw new SinPermisosException("No tiene permisos para registrar recepciones.");
    }
    // ...
}
```

### Validación en la Vista

```xml
<Button Content="Registrar Recepción"
        Command="{Binding RegistrarRecepcionAsync}"
        IsEnabled="{Binding PuedeRegistrarRecepcion}" />
```

---

## 7. Reglas de Seguridad

1. **Principio de Mínimo Privilegio**: Los usuarios solo tienen acceso a los módulos necesarios para su rol.
2. **Validación en Servidor**: Todas las operaciones son validadas en el servidor, no solo en el cliente.
3. **Tokens de Autenticación**: JWT con expiración para acceso a la API.
4. **Auditoría**: Todas las acciones quedan registradas en la bitácora.
5. **Separación de Responsabilidades**: Los administradores no tienen acceso automático a todas las funciones sensibles sin justificación.

---

## 8. Ejemplos de Escenarios

### Escenario 1: Bodegador intentando acceder a Usuarios
- El bodegador intenta navegar a "Usuarios y Permisos"
- El botón no está visible en el menú (oculto por falta de permiso)
- Si intenta acceder directamente por URL, recibe error de autorización

### Escenario 2: Supervisor intentando registrar ajuste
- El supervisor intenta registrar un ajuste de inventario
- El botón "Registrar Ajuste" está deshabilitado
- Si intenta ejecutar la acción, recibe mensaje de error de permisos

### Escenario 3: Auditor consultando bitácora
- El auditor puede ver la bitácora completa
- No puede marcar alertas como atendidas
- No puede realizar ajustes ni recepciones

---

## 9. Configuración de Permisos

Los permisos se configuran en la API central y se sincronizan con la aplicación cliente. No se almacenan localmente en la aplicación de escritorio.

Para cambiar los permisos de un usuario:
1. Acceder a "Usuarios y Permisos" (solo administrador)
2. Seleccionar el usuario
3. Modificar los roles y permisos asignados
4. Guardar cambios

---

## 10. Auditoría de Permisos

Todas las operaciones están registradas en la bitácora con:
- Usuario que realizó la acción
- Acción realizada
- Módulo afectado
- Fecha y hora
- Resultado (éxito o error)

Esto permite auditoría completa de todas las operaciones realizadas en el sistema.
