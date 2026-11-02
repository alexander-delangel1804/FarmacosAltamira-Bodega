# Fase 1 — Diseño Arquitectónico del Módulo de Bodega WPF

> **Proyecto:** Sistema Integral de Gestión y Centralización de Información para Fármacos Altamira  
> **Módulo:** Bodega e Inventario (Aplicación de escritorio Windows, WPF)  
> **Estado:** Diseño listo para implementación (pendiente de autorización del usuario para crear proyectos)

---

## 1. Diagrama de Arquitectura (Capas)

```
┌─────────────────────────────────────────────────────────────────────┐
│                     PRESENTACIÓN (WPF / MVVM)                       │
│  ┌─────────────┐  ┌──────────────────┐  ┌───────────────────────┐  │
│  │   Views     │  │   ViewModels     │  │   Modelos de vista    │  │
│  │  (XAML)     │  │  (CommunityToolkit│  │   (DTOs de presentación)│
│  └──────┬──────┘  └────────┬─────────┘  └───────────┬───────────┘  │
│         │                  │                         │              │
│         │   DataBinding    │     Comandos /          │              │
│         │   (TwoWay)       │     Notificación        │              │
│         ▼                  ▼                         ▼              │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │  Converters · Estilos (Material Design) · Recursos XAML       │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              │ Referencia a Interfaces de aplicación
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     APLICACIÓN (Servicios y Casos de uso)           │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐  │
│  │  Servicios de    │  │  DTOs de entrada │  │  Validadores     │  │
│  │  negocio (C#)    │  │  y salida        │  │  (FluentValidation)│
│  └────────┬─────────┘  └────────┬─────────┘  └────────┬─────────┘  │
│         │                       │                       │          │
│         │ Llama a               │ Retorna               │ Valida   │
│         ▼                       ▼                       ▼          │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              │ Referencia a Interfaces de dominio/infraestructura
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      DOMINIO (Entidades y Enums)                    │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐  │
│  │  Entidades       │  │  Enumeraciones   │  │  Excepciones     │  │
│  │  (puras, sin     │  │  (TipoMovimiento,│  │  de negocio      │  │
│  │   dependencias   │  │   EstadoLote, etc│  │  (ReglaNegocio)   │  │
│  └──────────────────┘  └──────────────────┘  └──────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              │ Implementa
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                   INFRAESTRUCTURA (Acceso a API)                    │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │  HttpClient (IHttpClientFactory)                              │  │
│  │  Clientes de API REST (Auth, Inventario, Traslados, etc.)     │  │
│  │  Manejador de tokens JWT (almacenamiento seguro en memoria)   │  │
│  │  Gestión de sesión y expiración                               │  │
│  │  Serialización JSON (camelCase)                               │  │
│  │  Mapa de errores de API → Excepciones de dominio              │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTPS + JSON (JWT Bearer)
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│              API CENTRAL (ASP.NET Core — ya existente)              │
│  PostgreSQL central — Acceso EXCLUSIVO mediante la API              │
│  (El cliente WPF NUNCA se conecta directamente a PostgreSQL)        │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 2. Diagrama de Dependencias entre Proyectos

```
Altamira.Bodega.Presentation  →  Altamira.Bodega.Application
Altamira.Bodega.Application   →  Altamira.Bodega.Domain
Altamira.Bodega.Application   →  Altamira.Bodega.Infrastructure
Altamira.Bodega.Infrastructure→  Altamira.Bodega.Domain
Altamira.Bodega.Tests         →  Todos los anteriores (según capa de prueba)
Altamira.Bodega.App           →  Altamira.Bodega.Presentation
                                Altamira.Bodega.Application
                                Altamira.Bodega.Infrastructure
```

**Regla:** Las dependencias fluyen hacia capas internas. Presentation nunca habla directamente con Infrastructure.

---

## 3. Estructura de Carpetas y Proyectos

### Solución: `Altamira.Bodega.sln`

```
Bodega/
├── Altamira.Bodega.sln
├── Directory.Build.props
├── Directory.Packages.props
├── src/
│   ├── Altamira.Bodega.App/              (Punto de entrada WPF, App.xaml, DI)
│   ├── Altamira.Bodega.Presentation/     (Views, ViewModels, Converters, Estilos)
│   ├── Altamira.Bodega.Application/      (Servicios de aplicación, DTOs, Validadores, Interfaces)
│   ├── Altamira.Bodega.Domain/           (Entidades, Enums, Excepciones de dominio, Interfaces de repositorio)
│   ├── Altamira.Bodega.Infrastructure/   (Clientes de API, HttpClient, Serilog, Configuración, Mapa de errores)
│   └── Altamira.Bodega.Tests/            (Pruebas xUnit)
├── docs/
│   └── 01_Diseno_Arquitectonico.md
└── installer/                            (WiX Toolset — pendiente Fase posterior)
```

### Detalle por proyecto

| Proyecto | Responsabilidad | Referencias |
|---|---|---|
| `Altamira.Bodega.Domain` | Entidades puras, enums, excepciones de negocio, interfaces de repositorio/servicio de dominio. Sin dependencias de NuGet salvo `System.Text.Json` annotations opcionales. | — |
| `Altamira.Bodega.Application` | Casos de uso, servicios de aplicación, DTOs de entrada/salida, validadores (FluentValidation), interfaces de servicios de aplicación. Depende de Domain. | Domain, Contracts (compartidos si existen) |
| `Altamira.Bodega.Infrastructure` | Implementación de clientes de API REST, `HttpClient` mediante `IHttpClientFactory`, manejo de tokens JWT, serialización, mapeo de respuestas de API a DTOs, Serilog. Depende de Domain y Application. | Domain, Application |
| `Altamira.Bodega.Presentation` | Views XAML, ViewModels (CommunityToolkit.Mvvm), comandos, converters, estilos Material Design, recursos XAML. Depende de Application y Domain (solo para tipos de presentación). | Application, Domain, CommunityToolkit.Mvvm, MaterialDesignInXamlToolkit |
| `Altamira.Bodega.App` | `App.xaml`, punto de entrada, registro de DI (`Microsoft.Extensions.Hosting`), configuración de Serilog, configuración de Material Design, inicialización de la aplicación. | Todos los anteriores |
| `Altamira.Bodega.Tests` | Pruebas unitarias xUnit. | Todos (según capa) |

---

## 4. Paquetes NuGet Obligatorios

| Paquete | Proyecto | Propósito |
|---|---|---|
| `CommunityToolkit.Mvvm` | Presentation, App | MVVM (ObservableObject, RelayCommand, ICommunityToolkit) |
| `MaterialDesignInXamlToolkit` | Presentation | Estilos y componentes visuales empresariales |
| `MaterialDesignThemes.Wpf` | Presentation | Temas de Material Design |
| `Microsoft.Extensions.Hosting` | App | Host genérico, DI |
| `Microsoft.Extensions.DependencyInjection` | App | Contenedor DI |
| `Microsoft.Extensions.Configuration` | App | Configuración por entorno (appsettings.json) |
| `Microsoft.Extensions.Http` | Infrastructure | `IHttpClientFactory` |
| `Serilog` | Infrastructure, App | Registros estructurados |
| `Serilog.AspNetCore` / `Serilog.Sinks.File` | Infrastructure, App | Sinks de consola y archivo |
| `FluentValidation` | Application | Validadores de DTOs de entrada |
| `System.Text.Json` | Infrastructure, Application | Serialización JSON (camelCase) |
| `xunit` | Tests | Framework de pruebas |
| `coverlet.collector` | Tests | Cobertura |
| `WiX Toolset` | Installer (futuro) | Instalador de Windows |

---

## 5. Convenciones de Desarrollo

### Nomenclatura

| Elemento | Convención | Ejemplo |
|---|---|---|
| Clases de dominio | PascalCase, sustantivo | `Producto`, `Lote`, `MovimientoInventario` |
| Enums | PascalCase, sin sufijo | `TipoMovimientoInventario`, `EstadoLote` |
| Excepciones de dominio | Sufijo `Exception` | `ReglaNegocioException`, `ExistenciaInsuficienteException` |
| Interfaces de servicio | `I` + nombre del servicio | `IInventarioService`, `IAuthService` |
| Implementación de servicio | Nombre del servicio (sin `I`) | `InventarioService`, `AuthService` |
| DTOs de entrada | Sufijo `Request` o `CreateDto`/`UpdateDto` | `ProductoCreateRequest`, `TrasladoSolicitarRequest` |
| DTOs de salida/respuesta | Sufijo `Dto` o `Response` | `ProductoDto`, `ExistenciaDto`, `LoginResponse` |
| ViewModels | Sufijo `ViewModel` | `LoginViewModel`, `InventarioViewModel` |
| Views | Sufijo `View` (o nombre de pantalla) | `LoginView`, `InventarioView`, `DashboardView` |
| Validadores | Sufijo `Validator` | `ProductoCreateValidator`, `LoginRequestValidator` |
| Clientes de API | Sufijo `ApiClient` o `Service` | `InventarioApiClient`, `AuthApiClient` |
| Comandos | `RelayCommand` / `AsyncRelayCommand` | `IniciarSesionCommand`, `BuscarProductosCommand` |
| Recursos XAML | `x:Key` en PascalCase descriptivo | `MaterialDesignCard`, `BotonPrimario`, `TextoAlerta` |

### Capas y responsabilidades

1. **Presentation (WPF):** Solo interacción de usuario, DataBinding, comandos, validación visual de formulario, navegación. NINGUNA lógica de negocio ni llamadas HTTP directas.
2. **Application:** Orquestración de casos de uso, validaciones de negocio (FluentValidation), transformación de DTOs, llamadas a servicios de infraestructura. NINGUNA llamada HTTP directa (delega en Infrastructure).
3. **Domain:** Entidades anémicas o ricas con reglas de dominio puras, enums, excepciones de negocio. Sin dependencias externas.
4. **Infrastructure:** Implementación concreta de acceso a API REST, manejo de tokens, serialización, logging, mapeo de errores HTTP a excepciones de dominio.

### MVVM con CommunityToolkit.Mvvm

- ViewModels heredan de `ObservableObject`.
- Propiedades usan `[ObservableProperty]`.
- Comandos usan `[RelayCommand]` / `[AsyncRelayCommand]`.
- Validación de propiedades con `IDataErrorInfo` o validación manual en el setter.
- Navegación mediante un `INavigationService` definido en Application e implementado en Presentation.

---

## 6. Estrategia de Comunicación con la API

### Base de clientes

```csharp
// Infrastructure/Clients/BaseApiClient.cs
public abstract class BaseApiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ITokenService _tokenService;
    protected readonly ILogger<BaseApiClient> _logger;

    // Agrega automáticamente el header Authorization: Bearer <token>
    // Maneja 401 → expira sesión, 403 → sin permisos, 409 → conflicto, 422 → regla de negocio
}
```

### Endpoints existentes de la API que consumirá el módulo Bodega

| Endpoint | Método | Uso en Bodega | Política de la API |
|---|---|---|---|
| `api/v1/auth/login` | POST | Autenticación de empleado de bodega | Público (rate limit) |
| `api/v1/auth/logout` | POST | Cierre de sesión | Autorizado |
| `api/v1/productos` | GET | Catálogo de productos | `BODEGA:PRODUCTOS` |
| `api/v1/productos/{id}` | GET | Detalle de producto | `BODEGA:PRODUCTOS` |
| `api/v1/productos` | POST | Crear producto | `BODEGA:PRODUCTOS` |
| `api/v1/productos/{id}` | PUT | Actualizar producto | `BODEGA:PRODUCTOS` |
| `api/v1/productos/{id}/stock-minimo/{idUbicacion}` | PUT | Actualizar stock mínimo | `BODEGA:PRODUCTOS` |
| `api/v1/categorias` | GET | Listar categorías | `BODEGA:PRODUCTOS` |
| `api/v1/inventario` | GET | Consultar existencias (paginado) | `BODEGA:CONSULTAR` |
| `api/v1/inventario/productos/{id}/existencias` | GET | Existencias por producto | `POS:CONSULTAR_STOCK` |
| `api/v1/lotes` | GET | Listar lotes | `BODEGA:CONSULTAR` |
| `api/v1/recepciones` | POST | Registrar recepción | `BODEGA:RECEPCION` |
| `api/v1/recepciones/{id}` | GET | Detalle de recepción | `BODEGA:RECEPCION` |
| `api/v1/ajustes` | POST | Registro de ajuste/merma | `BODEGA:AJUSTE` |
| `api/v1/movimientos` | GET | Consultar kárdex | `BODEGA:CONSULTAR` |
| `api/v1/traslados` | POST | Solicitar traslado | `BODEGA:TRASLADO` |
| `api/v1/traslados` | GET | Listar traslados | `BODEGA:CONSULTAR` |
| `api/v1/traslados/{id}` | GET | Detalle de traslado | `BODEGA:CONSULTAR` |
| `api/v1/traslados/{id}/despachar` | POST | Despachar traslado | `BODEGA:TRASLADO` |
| `api/v1/traslados/{id}/recibir` | POST | Recibir traslado | `BODEGA:TRASLADO` |
| `api/v1/traslados/{id}/cancelar` | POST | Cancelar traslado | `BODEGA:TRASLADO` |
| `api/v1/alertas` | GET | Listar alertas | `BODEGA:CONSULTAR` |
| `api/v1/alertas/{id}/atender` | POST | Marcar alerta atendida | `BODEGA:CONSULTAR` |
| `api/v1/usuarios` | GET | Listar usuarios | `SEGURIDAD:USUARIOS` |
| `api/v1/roles` | GET | Listar roles y permisos | `SEGURIDAD:USUARIOS` |
| `api/v1/parametros` | GET | Parámetros del sistema | `SEGURIDAD:PARAMETROS` |
| `api/v1/sync/existencias` | GET | Existencias de la ubicación (sync) | — |
| `api/v1/sync/bajar` | GET | Descargar catálogo/existencias | — |

**Nota:** Las políticas de autorización de la API (`[Authorize(Policy="...")]`) están declaradas pero **no registradas** en `services.AddAuthorization()` (hallazgo C-01 de Fase 0). Esto es un problema del backend que debe corregirse por separado; el cliente WPF debe manejar respuestas 403 adecuadamente.

### Gestión de sesión y tokens

- Token JWT almacenado en memoria (no en disco), propiedades: `AccessToken`, `RefreshToken`, `Expiracion`, `IdUsuario`, `Rol`, `IdUbicacion`, `IdCaja`, `Permisos`.
- `ITokenService`: almacena/obtiene/limpia token, valida expiración, extrae claims.
- Al recibir 401 de la API: limpiar sesión, redirigir a login, mostrar notificación.
- El WPF **no** almacena contraseñas ni credenciales de PostgreSQL.

### Manejo global de errores

- `ErrorHandlerService` en Infrastructure: intercepta excepciones de `HttpRequestException`, mapea códigos de estado HTTP a excepciones de dominio:
  - 400 → `DatosInvalidosException`
  - 401 → `SesionExpiradaException`
  - 403 → `SinPermisosException`
  - 404 → `NoEncontradoException`
  - 409 → `ConflictoConcurrenciaException` / `DuplicadoException`
  - 422 → `ReglaNegocioException`
  - 500 → `ErrorInternoException`
- Las Views capturan excepciones de los ViewModels y muestran notificaciones (Material Design Snackbar / MessageBox).

---

## 7. Configuración por Entorno

### `appsettings.json` (base)

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSegundos": 30
  },
  "Jwt": {
    "Issuer": "FarmacosAltamira",
    "Audience": "FarmacosAltamiraApi"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs\\bodega-.txt", "rollingInterval": "Day" } }
    ]
  }
}
```

### `appsettings.Development.json` / `appsettings.Production.json`

- Sobrescriben `Api:BaseUrl` según entorno.
- `Production`: HTTPS obligatorio, `MinimumLevel: Warning`, sink de archivo con rotación.

### Carga de configuración

```csharp
// Altamira.Bodega.App / Program.cs
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    ...
```

---

## 8. Plan de Implementación (Fases internas de Fase 1)

| Sub-fase | Objetivo | Archivos clave | Criterios de aceptación |
|---|---|---|---|
| 1.1 | Crear solución y proyectos | `.sln`, `.csproj`, `Directory.Build.props`, `Directory.Packages.props` | `dotnet build` exitoso, sin advertencias de dependencias circulares |
| 1.2 | Configurar DI, Serilog, HttpClient, Material Design | `App.xaml`, `App.xaml.cs`, `Program.cs`, `DependencyInjection.cs` | Aplicación inicia, ventana principal vacía se muestra, logs funcionan |
| 1.3 | Crear capa Domain (entidades, enums, excepciones, interfaces) | `Domain/**` | Compila sin dependencias de Presentation/Infrastructure |
| 1.4 | Crear capa Application (DTOs, validadores, interfaces de servicio) | `Application/**` | Compila, referencias correctas a Domain |
| 1.5 | Crear capa Infrastructure (clientes API, token service, error handler) | `Infrastructure/**` | Compila, `HttpClient` tipado, serialización camelCase |
| 1.6 | Crear base de Presentation (NavigationService, ViewModel base, estilos) | `Presentation/**` | Material Design aplicado, navegación básica funcional |
| 1.7 | Implementar login y sesión | Views/ViewModels de login, AuthApiClient | Login exitoso guarda token, logout limpia sesión, expiración redirige a login |
| 1.8 | Implementar dashboard y navegación lateral | DashboardView, ShellView, menú lateral | Dashboard carga, navegación entre pantallas funciona |
| 1.9 | Pruebas unitarias iniciales | `Tests/**` | xUnit ejecuta, al menos 1 prueba por capa crítica |

---

## 9. Modelo de Datos de Presentación (DTOs del cliente)

Los DTOs del cliente WPF se mapean 1:1 a los DTOs de respuesta de la API para evitar acoplamiento. Ejemplos:

```csharp
// Application/Dtos/ProductoDto.cs (cliente)
public sealed record ProductoDto(int IdProducto, string CodigoBarras, string NombreComercial, ...);

// Application/Dtos/ExistenciaDto.cs (cliente)
public sealed record ExistenciaDto(int IdProducto, string ProductoNombre, int IdLote, string NumeroLote, DateOnly FechaCaducidad, int Cantidad, int CantidadReservada, int Disponible, ...);

// Application/Dtos/LoginResponse.cs (cliente)
public sealed record LoginResponse(string AccessToken, string RefreshToken, int IdUsuario, string Rol, int? IdUbicacion, DateTimeOffset Expiracion);
```

---

## 10. Riesgos Identificados y Mitigación

| Riesgo | Impacto | Mitigación |
|---|---|---|
| Políticas de autorización no registradas en API (C-01) | Endpoints protegidos devuelven 403 | Cliente maneja 403 con mensaje claro; reportar al equipo backend |
| Credenciales hardcodeadas en appsettings de la API | Seguridad | Cliente WPF nunca almacena credenciales de BD; solo token JWT en memoria |
| Workers de la API comentados (alertas, respaldo) | Alertas no se generan automáticamente | Cliente consulta endpoint `/alertas`; generación de alertas es responsabilidad del backend |
| No hay pruebas en la API | Regresiones posibles | Escribir pruebas de integración en `Altamira.Bodega.Tests` contra endpoints reales o mock |

---

**Estado:** Diseño completo. Pendiente autorización para crear la solución y proyectos.