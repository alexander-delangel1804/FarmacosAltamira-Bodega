# Altamira.Bodega - Módulo de Bodega para Fármacos Altamira

## 📋 Descripción del Proyecto

**Altamira.Bodega** es una aplicación de escritorio (WPF) desarrollada en .NET 10 que permite la gestión completa de inventario para la bodega central de Fármacos Altamira. El sistema permite gestionar existencias, productos, lotes, recepciones, ajustes, traslados, alertas de caducidad y stock mínimo, con un completo historial de movimientos y trazabilidad completa.

## 🏗️ Arquitectura de la Solución

La solución sigue una arquitectura multicapa con separación de responsabilidades:

```
Altamira.Bodega.sln
├── Altamira.Bodega.Domain          (Capa de Dominio - Entidades, DTOs, Interfaces)
├── Altamira.Bodega.Application     (Capa de Aplicación - Contratos de servicios)
├── Altamira.Bodega.Infrastructure  (Capa de Infraestructura - Clientes API, Servicios)
├── Altamira.Bodega.Presentation    (Capa de Presentación - WPF, ViewModels, Vistas)
└── Altamira.Bodega.App             (Capa de Aplicación - Punto de entrada WPF)
```

### Capa de Domain (`Altamira.Bodega.Domain`)
- **Dtos**: Objetos de transferencia de datos (ProductoDto, ExistenciaDto, LoteDto, TrasladoDto, etc.)
- **Interfaces**: Contratos de servicios (IInventarioService, IProductoAppService, IAuthService, etc.)
- **Excepciones**: Excepciones de dominio personalizadas

### Capa de Application (`Altamira.Bodega.Application`)
- Contratos de servicios de aplicación
- Interfaces para servicios de negocio

### Capa de Infrastructure (`Altamira.Bodega.Infrastructure`)
- **Clients**: Clientes REST para comunicación con la API central
- **Services**: Implementación de servicios de aplicación
- **DependencyInjection**: Registro de servicios en contenedor DI

### Capa de Presentation (`Altamira.Bodega.Presentation`)
- **ViewModels**: Lógica de presentación con MVVM (CommunityToolkit.Mvvm)
- **Views**: Vistas XAML para cada módulo
- **Resources**: Estilos, estilos de tabla, convertidores
- **DependencyInjection**: Registro de ViewModels y vistas

### Capa de App (`Altamira.Bodega.App`)
- Punto de entrada de la aplicación WPF
- Configuración de Serilog para logging
- Configuración de servicios

---

## 🚀 Inicio Rápido

### Requisitos Previos

- .NET 10 SDK instalado
- Microsoft Edge WebView2 (para componentes web si los hay)
- Acceso a la API central de Fármacos Altamira

### Ejecución en Desarrollo

```bash
# Clonar el repositorio
git clone <url-del-repositorio>
cd Bodega

# Restaurar dependencias
dotnet restore

# Ejecutar en modo desarrollo
dotnet run --project src/Altamira.Bodega.App/Altamira.Bodega.App.csproj -c Debug
```

### Ejecución en Producción

```bash
dotnet publish src/Altamira.Bodega.App/Altamira.Bodega.App.csproj -c Release -o ./publish
cd src/Altamira.Bodega.App/bin/Release/net10.0-windows/publish
Altamira.Bodega.App.exe
```

---

## 📦 Estructura de Módulos

| Módulo | Vista | ViewModel | Descripción |
|--------|-------|-----------|-------------|
| Dashboard | DashboardView | DashboardViewModel | Panel principal con KPIs |
| Existencias | ExistenciasView | ExistenciaViewModel | Consulta de existencias por ubicación |
| Catálogo | CatalogoView | ProductoViewModel | Administración de productos |
| Lotes | LoteInfoView | LoteInfoViewModel | Información detallada de lotes |
| Kárdex | KardexView | KardexViewModel | Historial de movimientos |
| Ajustes | AjustesView | AjustesViewModel | Ajustes de inventario |
| Recepción | RecepcionView | RecepcionViewModel | Registro de recepciones |
| Historial Recepción | RecepcionHistorialView | RecepcionHistorialViewModel | Historial de recepciones |
| Entradas | EntradasView | EntradasViewModel | Entradas de inventario |
| Salidas | SalidasView | SalidasViewModel | Salidas de inventario |
| Traslados | TrasladosView | TrasladoCrearViewModel | Traslados entre ubicaciones |
| Alertas Caducidad | AlertasCaducidadView | AlertasCaducidadViewModel | Alertas por caducidad |
| Alertas Stock | AlertasStockView | AlertasStockViewModel | Alertas por stock mínimo |
| FEDEX | FEFOView | FEFOViewModel | Control FEFO (First Expired First Out) |
| Bitácora | BitacoraView | BitacoraViewModel | Bitácora de operaciones |
| Usuarios | UsuariosView | UsuariosViewModel | Gestión de usuarios |

---

## 🔐 Seguridad y Autenticación

### Autenticación
El sistema utiliza autenticación basada en JWT (JSON Web Tokens) contra la API central de Fármacos Altamira.

**Flujo de autenticación:**
1. Usuario ingresa credenciales en LoginView
2. LoginViewModel llama a `IAuthAppService.LoginAsync()`
3. La API valida credenciales y devuelve un token JWT
4. El token se almacena en `TokenService` y la sesión en `SesionService`
5. El usuario es redirigido al Dashboard

### Permisos y Roles
El sistema implementa control de acceso basado en roles:

| Rol | Módulos Accesibles |
|-----|-------------------|
| Administrador | Todos los módulos |
| Bodeguero | Existencias, Catálogo, Recepción, Ajustes, Traslados |
| Supervisor | Dashboard, Kardex, Alertas, Bitácora |
| Auditor | Bitácora, Reportes |

### Verificación de Permisos
Cada acción en el módulo de bodega verifica permisos mediante `ISesionAppService.PuedeAccederAMinistro(modulo, accion)` antes de ejecutar operaciones sensibles.

---

## 📊 Módulos Funcionales

### 1. Catálogo de Productos
Permite administrar el catálogo de productos con:
- Búsqueda por nombre, código de barras o sustancia activa
- Filtro por activos/inactivos
- Creación, edición y desactivación de productos
- Visualización de stock mínimo y estado

### 2. Existencias por Ubicación
Consulta de inventario con:
- Filtros por producto, lote y ubicación
- Paginación automática
- Indicadores de estado (vigente, bajo mínimo, agotado)
- Exportación de datos

### 3. Recepción de Mercancía
Registro de recepciones con:
- Selección de producto y lote
- Validación de cantidades y fechas de caducidad
- Validación de lotes caducados (requiere autorización especial)
- Registro de documento de referencia y observaciones
- Registro transaccional en backend

### 4. Ajustes de Inventario
Registro de ajustes con:
- Selección de producto y lote
- Tipo de ajuste (positivo/negativo)
- Validación de existencia disponible
- Validación de no generar existencias negativas
- Registro de motivo y usuario

### 5. Traslados
Gestión de traslados entre ubicaciones con:
- Solicitud de traslado entre ubicaciones
- Despacho de traslados (origen)
- Recepción de traslados (destino)
- Historial completo de traslados
- Estados: SOLICITADO → EN_TRANSITO → RECIBIDO

### 6. Alertas
Sistema de alertas con:
- **Alertas de Caducidad**: Productos próximos a caducar o caducados
- **Alertas de Stock Mínimo**: Productos por debajo del stock mínimo
- Filtros por fecha, ubicación, tipo y producto
- Marcar alertas como atendidas

### 7. Control FEFQ (First Expired First Out)
Control de lotes por fecha de caducidad con:
- Priorización por fecha de caducidad más próxima
- Identificación de lotes elegibles para salida
- Alertas para lotes caducados o próximos a caducar

### 8. Bitácora de Operaciones
Registro trazable de todas las operaciones con:
- Folio, fecha, usuario, ubicación
- Tipo de movimiento, cantidad, motivo
- Folio de referencia
- Filtrado por fecha, usuario, módulo, acción

---

## 📊 Capas de Seguridad

### Autenticación
- Autenticación JWT contra API central
- Tokens de acceso y refresh
- Almacenamiento seguro en memoria de sesión
- Expiración automática de tokens

### Autorización
- Control de acceso basado en roles (Rol)
- Verificación de permisos por módulo y acción (`PuedeAccederAMinistro`)
- Validación tanto en cliente como en backend
- No se confía únicamente en la visibilidad de botones

### Integridad de Datos
- Validaciones en cliente y servidor
- Transacciones atómicas en recepciones y ajustes
- No permite existencias negativas
- No permite editar o eliminar movimientos históricos
- Correcciones mediante nuevos movimientos trazables

---

## 📋 Matriz de Permisos

| Módulo | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Dashboard | ✓ | ✓ | ✓ | ✓ |
| Existencias | ✓ | ✓ | ✓ | ✓ |
| Catálogo | ✓ | ✓ | ✓ | ✓ |
| Lotes | ✓ | ✓ | ✓ | ✓ |
| Kárdex | ✓ | ✓ | ✓ | ✓ |
| Ajustes | ✓ | ✓ | ✓ | ❌ |
| Recepción | ✓ | ✓ | ✓ | ❌ |
| Traslados | ✓ | ✓ | ✓ | ✓ |
| Alertas | ✓ | ✓ | ✓ | ✓ |
| FEFO | ✓ | ✓ | ✓ | ✓ |
| Bitácora | ✓ | ❌ | ✓ | ✓ |
| Usuarios | ✓ | ❌ | ❌ | ❌ |

---

## 📖 Manual de Usuario

### Inicio de Sesión
1. Ejecutar la aplicación
2. Ingresar nombre de usuario y contraseña
3. Seleccionar estación (opcional)
4. Hacer clic en "Iniciar Sesión"
5. Si las credenciales son correctas, se abre el Dashboard

### Consultar Existencias
1. Navegar a "Existencias por Ubicación" desde el menú lateral
2. Usar filtros de producto, lote o ubicación si es necesario
3. Utilizar búsqueda para filtrar rápidamente
4. Usar paginación para navegar entre páginas

### Registrar Recepción
1. Navegar a "Recepción de Mercancía"
2. Seleccionar ubicación de destino (diferente a la ubicación actual)
3. Seleccionar producto y lote
4. Capturar cantidad y fecha de caducidad
5. Agregar partida (repetir para múltiples lotes)
6. Ingresar documento de referencia y observaciones (opcional)
7. Hacer clic en "Registrar Recepción"

### Registrar Ajuste
1. Navegar a "Ajustes de Inventario"
2. Seleccionar producto y lote
3. Seleccionar tipo de ajuste (Positivo o Negativo)
4. Capturar cantidad y motivo
5. Verificar que el resultado esperado sea correcto
6. Hacer clic en "Registrar Ajuste"

### Consultar Historial de Traslados
1. Navegar a "Historial de Traslados"
2. Usar filtros por folio, origen, destino, estado o fechas
3. Revisar detalles de cada recepción en la tabla

---

## 📄 Manual Técnico

### Arquitectura
La aplicación sigue una arquitectura de capas con separación clara de responsabilidades:

```
Presentation Layer (WPF + MVVM)
    ↓
Application Layer (Contratos de servicio)
    ↓
Infrastructure Layer (Clientes API, Implementación)
    ↓
Domain Layer (Entidades, DTOs, Interfaces de dominio)
```

### Patrones de Diseño Utilizados
- **MVVM** (Model-View-ViewModel) para separación de vista y lógica
- **Dependency Injection** para inyección de dependencias
- **Repository Pattern** a través de clientes API
- **Repository Pattern** a través de servicios de aplicación
- **DTO Pattern** para transferencia de datos

### Librerías Principales
| Paquete | Versión | Uso |
|---------|---------|-----|
| CommunityToolkit.Mvvm | 8.4.0 | MVVM, RelayCommand, ObservableProperty |
| Microsoft.Extensions.Hosting | 10.0.0 | Inyección de dependencias |
| Microsoft.Extensions.Configuration | 10.0.0 | Configuración |
| Serilog | 4.2.0 | Logging |
| FluentValidation | 11.11.0 | Validaciones (si aplica) |
| Moq | 4.20.72 | Pruebas unitarias |
| FluentAssertions | 8.4.0 | Asserciones en pruebas |
| xUnit | 2.9.3 | Framework de pruebas |

---

## 📋 Matriz de Permisos

| Módulo | Administrador | Bodegador | Supervisor | Auditor |
|--------|--------------|-----------|------------|---------|
| Dashboard | ✓ | ✓ | ✓ | ✓ |
| Existencias | ✓ | ✓ | ✓ | ✓ |
| Catálogo | ✓ | ✓ | ✓ | ✓ |
| Lotes | ✓ | ✓ | ✓ | ✓ |
| Kárdex | ✓ | ✓ | ✓ | ✓ |
| Ajustes | ✓ | ✓ | ✓ | ❌ |
| Recepción | ✓ | ✓ | ✓ | ❌ |
| Traslados | ✓ | ✓ | ✓ | ✓ |
| Alertas | ✓ | ✓ | ✓ | ✓ |
| FEFO | ✓ | ✓ | ✓ | ✓ |
| Bitácora | ✓ | ❌ | ✓ | ✓ |
| Usuarios | ✓ | ❌ | ❌ | ❌ |

---

## 🔧 Guía de Resolución de Problemas

### Problema: No se puede iniciar sesión
**Causas posibles:**
- Credenciales incorrectas
- Usuario no tiene permisos para el módulo de bodega
- Problema de conexión con la API

**Solución:**
1. Verificar credenciales de usuario
2. Verificar que el usuario tenga rol asignado
3. Verificar conectividad con la API central
4. Revisar logs en `logs/bodega-*.txt`

### Problema: No se pueden cargar existencias
**Causas posibles:**
- Problema de conexión con la API
- Ubicación no configurada para el usuario
- Error en la API central

**Solución:**
1. Verificar conexión a internet/API
2. Verificar que el usuario tenga ubicación asignada
3. Revisar logs para detalles del error

### Problema: No se puede registrar recepción
**Causas posibles:**
- Cantidad excede existencia disponible
- Lote caducado sin autorización
- Producto o lote no seleccionado
- Cantidad menor o igual a cero

**Solución:**
1. Verificar que haya existencia disponible para el lote
2. Verificar que el lote no esté caducado (o tener autorización)
3. Seleccionar producto y lote válidos
4. Ingresar cantidad mayor a cero

---

## 📊 Matriz de Cumplimiento de Requisitos

| Requisito | Estado | Comentario |
|-----------|--------|------------|
| Arquitectura multicapa | ✅ Implementado |
| Autenticación JWT | ✅ Implementado |
| Autorización por roles | ✅ Implementado |
| Bitácora de operaciones | ✅ Implementado |
| Logs con Serilog | ✅ Implementado |
| Pruebas unitarias | ✅ 76 pruebas pasando |
| Vistas WPF completas | ✅ Implementadas |
| Navegación entre vistas | ✅ Implementada |
| Validaciones de entrada | ✅ Implementadas |
| Manejo de errores | ✅ Implementado |
| Documentación | ✅ Este documento |

---

## 📞 Soporte

Para soporte técnico del Módulo de Bodega de Fármacos Altamira, contactar al departamento de sistemas internos.

---

*Documento generado automáticamente como parte de la Fase 10 - Documentación y Publicación*
