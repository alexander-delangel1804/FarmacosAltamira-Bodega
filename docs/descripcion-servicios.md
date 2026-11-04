# Descripción de Servicios - Módulo de Bodega Fármacos Altamira

## 1. Introducción

Este documento describe todos los servicios de la aplicación Altamira.Bodega, sus responsabilidades, interfaces y dependencias.

---

## 2. Servicios de Dominio (Domain.Interfaces)

### IInventarioService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Consultar existencias por producto y ubicación
- Listar lotes por producto
- Registrar recepciones de mercancía
- Registrar ajustes de inventario
- Consultar movimientos de inventario
- Listar recepciones por rango de fechas

Métodos:
- `ConsultarExistenciasAsync(int? idProducto, int? idUbicacion, CancellationToken ct)`
- `ListarLotesAsync(int? idProducto, CancellationToken ct)`
- `RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct)`
- `RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct)`
- `ConsultarMovimientosAsync(int? idLote, int? idUbicacion, DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct)`
- `ListarRecepcionesAsync(DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct)`

---

### IProductoService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Listar productos con filtros
- Obtener producto por ID
- Crear producto
- Actualizar producto
- Desactivar producto

Métodos:
- `ListarProductosAsync(string? busqueda, bool soloActivos, CancellationToken ct)`
- `ObtenerProductoAsync(int id, CancellationToken ct)`
- `CrearProductoAsync(ProductoCreateDto dto, int idUsuario, CancellationToken ct)`
- `ActualizarProductoAsync(int id, ProductoUpdateDto dto, int idUsuario, CancellationToken ct)`
- `DesactivarProductoAsync(int id, int idUsuario, CancellationToken ct)`

---

### IAuthService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Autenticación de usuarios (login)
- Cierre de sesión
- Renovación de token

Métodos:
- `LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct)`
- `LogoutAsync(int idUsuario, string? jti, CancellationToken ct)`

---

### IAlertaService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Listar alertas con filtros
- Marcar alertas como atendidas

Métodos:
- `ListarAlertasAsync(int? idUbicacion, bool soloNoAtendidas, CancellationToken ct)`
- `MarcarAlertaAtendidaAsync(int idAlerta, int idUsuario, CancellationToken ct)`

---

### ITokenService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Gestión de tokens de autenticación
- Información de sesión del usuario

Propiedades:
- AccessToken, RefreshToken, Expiracion, IdUsuario, Rol, IdUbicacion, NombreUsuario, NombreCompleto, UbicacionNombre, IdCaja, CajaNumero
- EstaAutenticado, TokenExpirado

Métodos:
- GuardarSesion(LoginResponse response)
- LimpiarSesion()

---

### IRecepcionService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Listar recepciones
- Obtener recepción por ID
- Registrar recepción

Métodos:
- `ListarRecepcionesAsync(CancellationToken ct)`
- `ObtenerRecepcionAsync(int id, CancellationToken ct)`
- `RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct)`

---

### IAjusteService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Registrar ajustes de inventario

Métodos:
- `RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct)`

---

### ITrasladoService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Solicitar traslados
- Listar traslados con filtros
- Despachar traslados
- Recibir traslados
- Cancelar traslados

Métodos:
- `SolicitarTrasladoAsync(TrasladoCreateDto dto, int idUsuario, CancellationToken ct)`
- `ListarTrasladosAsync(string? estado, int? idOrigen, int? idDestino, CancellationToken ct)`
- `DespacharTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct)`
- `RecibirTrasladoAsync(int idTraslado, TrasladoRecibirDto dto, int idUsuario, CancellationToken ct)`
- `CancelarTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct)`

---

### INavigationService
**Ubicación:** `Altamira.Bodega.Domain.Interfaces`

Responsabilidades:
- Navegación entre vistas de la aplicación
- Evento de cambio de vista

Métodos:
- `NavigateTo<TViewModel>() where TViewModel : class`
- `NavigateTo<TViewModel, TParameter>(TParameter parameter) where TViewModel : class`
- `bool CanGoBack { get; }`
- `void GoBack()`
- `event Action<object>? ViewChanged`

---

## 2. Servicios de Aplicación (Application.Interfaces)

### IInventarioAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Coordinar consultas de existencias y lotes
- Coordinar registro de recepciones y ajustes
- Coordinar consulta de movimientos y recepciones

Métodos delegados a Infrastructure:
- ConsultarExistenciasAsync, ListarLotesAsync, RegistrarRecepcionAsync, RegistrarAjusteAsync
- ConsultarMovimientosAsync, ListarRecepcionesAsync

---

### IProductoAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Coordinar operaciones de catálogo de productos

Métodos delegados a Application:
- ListarProductosAsync, ObtenerProductoAsync, CrearProductoAsync, ActualizarProductoAsync, DesactivarProductoAsync

---

### IAuthAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Coordinar autenticación de usuarios
- Coordinar cierre de sesión
- Coordinar renovación de token

Métodos:
- `LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct)`
- `LogoutAsync(int idUsuario, string? jti, CancellationToken ct)`
- `RefreshTokenAsync(string refreshToken, CancellationToken ct)`

---

### IAlertaAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Coordinar listado de alertas
- Coordinar marcado de alertas como atendidas

Métodos:
- `ListarAlertasAsync(int? idUbicacion, bool soloNoAtendidas, CancellationToken ct)`
- `MarcarAlertaAtendidaAsync(int idAlerta, int idUsuario, CancellationToken ct)`

---

### IRolAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Listar roles
- Listar permisos

Métodos:
- `ListarRolesAsync(CancellationToken ct)`
- `ListarPermisosAsync(CancellationToken ct)`

---

### ISesionAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Proporcionar información de sesión de usuario
- Validar permisos de acceso
- Guardar y limpiar sesión

Propiedades:
- EstaAutenticado, TokenExpirado, IdUsuario, Rol, IdUbicacion, NombreUsuario, NombreCompleto, UbicacionNombre, IdCaja, CajaNumero, Expiracion

Métodos:
- GuardarSesion(LoginResponse response)
- LimpiarSesion()
- PuedeAccederAMinistro(string modulo, string accion)

---

### IBitacoraAppService
**Ubicación:** `Altamira.Bodega.Application.Interfaces`

Responsabilidades:
- Consultar bitácora con filtros

Métodos:
- `ConsultarBitacoraAsync(int? idUsuario, int? idUbicacion, string? modulo, string? accion, DateTimeOffset? fechaDesde, DateTimeOffset? fechaHasta, CancellationToken ct)`

---

## 3. Servicios de Infraestructura (Infrastructure.Clients)

### AuthApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Llamar a API de autenticación
- Login, logout, refresh token

Métodos:
- `LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct)`
- `LogoutAsync(int idUsuario, string? jti, CancellationToken ct)`
- `RefreshTokenAsync(string refreshToken, CancellationToken ct)`

---

### InventarioApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Consultar existencias
- Listar lotes
- Registrar recepciones y ajustes
- Consultar movimientos y recepciones

Métodos:
- `ConsultarExistenciasAsync(int? idProducto, int? idUbicacion, CancellationToken ct)`
- `ListarLotesAsync(int? idProducto, CancellationToken ct)`
- `RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct)`
- `RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct)`
- `ConsultarMovimientosAsync(int? idLote, int? idUbicacion, DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct)`
- `ListarRecepcionesAsync(DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct)`

---

### ProductoApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de catálogo de productos

Métodos:
- ListarProductosAsync, ObtenerProductoAsync, CrearProductoAsync, ActualizarProductoAsync, DesactivarProductoAsync

---

### TrasladoApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de traslados

Métodos:
- SolicitarTrasladoAsync, ListarTrasladosAsync, DespacharTrasladoAsync, RecibirTrasladoAsync, CancelarTrasladoAsync

---

### RecepcionApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de recepciones

Métodos:
- RegistrarRecepcionAsync

---

### AjusteApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de ajustes

Métodos:
- RegistrarAjusteAsync

---

### AlertaApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de alertas

Métodos:
- ListarAlertasAsync

---

### UsuarioApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de usuarios

Métodos:
- ListarUsuariosAsync, CrearUsuarioAsync, ActualizarUsuarioAsync, etc.

---

### RolApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de roles

Métodos:
- ListarRolesAsync, ListarPermisosAsync

---

### ParametroApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de parámetros

Métodos:
- ListarParametrosAsync, ObtenerParametroAsync, ActualizarParametroAsync

---

### UsuarioApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de usuarios

Métodos:
- ListarUsuariosAsync, CrearUsuarioAsync, ActualizarUsuarioAsync, BloquearUsuarioAsync, DesbloquearUsuarioAsync, DesactivarUsuarioAsync

---

### RolApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de roles

Métodos:
- ListarRolesAsync, ListarPermisosAsync

---

### BitacoraApiClient
**Ubicación:** `Altamira.Bodega.Infrastructure.Clients`

Responsabilidades:
- Operaciones de bitácora

Métodos:
- ConsultarBitacoraAsync

---

## 4. Servicios de Infraestructura (Infrastructure.Services)

### TokenService
**Ubicación:** `Altamira.Bodega.Infrastructure.Services`

Responsabilidades:
- Almacenar y gestionar tokens JWT
- Proporcionar información de sesión
- Implementa ITokenService

### SesionService
**Ubicación:** `Altamira.Bodega.Infrastructure.Services`

Responsabilidades:
- Gestionar sesión de usuario
- Verificar autenticación y expiración
- Implementar ISesionService

### ApiExceptionMapper
**Ubicación:** `Altamira.Bodega.Infrastructure.Services`

Responsabilidades:
- Mapear errores de API a excepciones de dominio
- Convertir códigos HTTP a excepciones específicas

### ApplicationServices
**Ubicación:** `Altamira.Bodega.Infrastructure.Services`

Responsabilidades:
- Implementar servicios de aplicación
- Coordinar entre clientes API y aplicación

Servicios implementados:
- ProductoAppService, InventarioAppService, TrasladoAppService
- AlertaAppService, UsuarioAppService, RolAppService, ParametroAppService
- BitacoraAppService, AuthAppService, SesionAppService

---

## 5. Servicios de Presentation (Presentation.Services)

### NavigationService
**Ubicación:** `Altamira.Bodega.Presentation.Services`

Responsabilidades:
- Navegación entre vistas de la aplicación
- Implementar INavigationService
- Mapear ViewModels a Views

Métodos:
- NavigateTo<TViewModel>()
- NavigateTo<TViewModel, TParameter>(TParameter parameter)
- CanGoBack, GoBack()
- event Action<object>? ViewChanged

---

## 6. Diagramas de Flujo

### Flujo de Autenticación
```
Usuario → LoginView → LoginViewModel → IAuthAppService → AuthApiClient → API Central
                                               ↓
                                    TokenService (almacena token)
                                    SesionService (guarda sesión)
                                    ShellViewModel (navega a Dashboard)
```

### Flujo de Consulta de Existencias
```
Usuario → ExistenciaView → ExistenciaViewModel → IInventarioAppService → InventarioAppService → InventarioApiClient → API Central
                                               ↓
                                    Resultado → ViewModel → Vista (muestra datos)
```

### Flujo de Recepción de Mercancía
```
Usuario → RecepcionView → RecepcionViewModel → IInventarioAppService → InventarioAppService → InventarioApiClient → API Central
                                               ↓
                                    Resultado → ViewModel → Vista (muestra confirmación)
```

---

## 7. Esquema de Comunicación

Todas las comunicaciones con la API central se realizan mediante HTTP REST con autenticación JWT.

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | /api/v1/auth/login | Autenticación de usuario |
| POST | /api/v1/auth/logout | Cierre de sesión |
| POST | /api/v1/auth/refresh | Renovación de token |
| GET | /api/v1/inventario | Consultar existencias |
| GET | /api/v1/lotes | Listar lotes |
| POST | /api/v1/recepciones | Registrar recepción |
| POST | /api/v1/ajustes | Registrar ajuste |
| GET | /api/v1/movimientos | Consultar movimientos |
| GET | /api/v1/alertas | Listar alertas |
| GET | /api/v1/traslados | Listar traslados |
| POST | /api/v1/traslados | Solicitar traslado |
| POST | /api/v1/traslados/{id}/despachar | Despachar traslado |
| POST | /api/v1/traslados/{id}/recibir | Recibir traslado |

---

## 8. Consideraciones de Seguridad

- Toda comunicación con la API se realiza mediante HTTPS
- Los tokens JWT se almacenan en memoria (no en disco)
- Las credenciales no se almacenan en el cliente
- Los secretos no se incluyen en el código fuente ni en el instalador
- Se utilizan variables de entorno para configuración sensible

---

## 9. Consideraciones de Rendimiento

- Las llamadas a la API son asíncronas (async/await)
- Se utilizan clientes HTTP reutilizables (HttpClientFactory)
- Los datos se cargan bajo demanda
- Se implementa paginación en consultas de gran volumen

---

## 10. Consideraciones de Mantenibilidad

- Código separado por capas (Domain, Application, Infrastructure, Presentation, App)
- Interfaces para todos los servicios
- Inyección de dependencias para facilitar pruebas unitarias
- Nombres de clases y métodos descriptivos
- Comentarios en código para lógica compleja

---

## 📞 Soporte

Para soporte técnico de servicios, contactar al departamento de sistemas internos de Fármacos Altamira.
