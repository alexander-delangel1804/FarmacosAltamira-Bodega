# Estructura de la Solución - Altamira.Bodega

## 1. Arquitectura General

El proyecto sigue una arquitectura multicapa con separación de responsabilidades clara. Cada capa tiene responsabilidades bien definidas y las dependencias fluyen en una sola dirección (de presentación hacia dominio).

```
Altamira.Bodega.sln
├── Altamira.Bodega.Domain          (Capa de Dominio)
├── Altamira.Bodega.Application     (Capa de Aplicación)
├── Altamira.Bodega.Infrastructure  (Capa de Infraestructura)
├── Altamira.Bodega.Presentation    (Capa de Presentación)
└── Altamira.Bodega.App             (Capa de Aplicación / Punto de entrada)
```

## 2. Descripción de Capas

### 2.1 Altamira.Bodega.Domain (Capa de Dominio)

Contiene las entidades del dominio, DTOs (Data Transfer Objects) e interfaces de servicios de dominio. No tiene dependencias externas y no depende de ninguna otra capa.

**Contenido:**
- **Dtos**: Objetos de transferencia de datos (ProductoDto, ExistenciaDto, LoteDto, TrasladoDto, AlertaDto, RecepcionDto, AjusteDto, UsuarioDto, LoginResponse, LoginRequest, etc.)
- **Interfaces**: Contratos de servicios de dominio (IInventarioService, IProductoService, IAuthService, IAlertaService, ITokenService, IRecepcionService, IAjusteService, ITrasladoService, INavigationService)
- **Excepciones**: Excepciones de dominio personalizadas (ReglaNegocioException, NotEncontradoException, etc.)

**Responsabilidades:**
- Definir las entidades de negocio
- Definir los contratos de servicios que las capas superiores implementarán
- Definir los DTOs para transferencia de datos entre capas

---

### 2.2 Altamira.Bodega.Application (Capa de Aplicación)

Contiene las interfaces de servicios de aplicación que definen los contratos de negocio. No contiene implementación, solo contratos.

**Contenido:**
- Interfaces de servicios de aplicación (IInventarioAppService, IProductoAppService, IAuthAppService, etc.)
- Definición de contratos para operaciones de negocio

**Responsabilidades:**
- Definir contratos de servicios de aplicación
- Separar la definición de contratos de la implementación
- Proporcionar una capa de abstracción entre Domain e Infrastructure

---

### 2.3 Altamira.Bodega.Infrastructure (Capa de Infraestructura)

Contiene la implementación de clientes REST para comunicación con la API central, implementación de servicios de aplicación y configuración de inyección de dependencias.

**Contenido:**
- **Clients**: Clientes REST para comunicación con la API central (AuthApiClient, InventarioApiClient, ProductoApiClient, TrasladoApiClient, RecepcionApiClient, AjusteApiClient, AlertaApiClient, UsuarioApiClient, RolApiClient, ParametroApiClient, BitacoraApiClient)
- **Services**: Implementación de servicios de aplicación (ProductoAppService, InventarioAppService, TrasladoAppService, AlertaAppService, UsuarioAppService, RolAppService, ParametroAppService, BitacoraAppService, AuthAppService, SesionAppService)
- **DependencyInjection**: Registro de servicios en contenedor DI (InfrastructureDependencyInjection)

**Responsabilidades:**
- Implementar clientes REST para comunicación con API
- Implementar servicios de aplicación
- Configurar inyección de dependencias
- Manejar comunicación con API central

---

### 2.4 Altamira.Bodega.Presentation (Capa de Presentación)

Implementa la interfaz de usuario usando WPF (Windows Presentation Foundation) con el patrón MVVM (Model-View-ViewModel).

**Contenido:**
- **ViewModels**: Lógica de presentación con MVVM (CommunityToolkit.Mvvm)
- **Views**: Vistas XAML para cada módulo
- **Resources**: Estilos, estilos de tabla, convertidores
- **DependencyInjection**: Registro de ViewModels y vistas (PresentationDependencyInjection)
- **Services**: Servicios de presentación (NavigationService)

**ViewModels:**
- ShellViewModel: Navegación principal y estado de sesión
- DashboardViewModel: KPIs y resumen del dashboard
- ExistenciaViewModel: Gestión de existencias con filtros y paginación
- ProductoViewModel: Catálogo de productos con búsqueda y CRUD
- LoteInfoViewModel: Información detallada de lotes
- RecepcionViewModel: Recepción de mercancía con validaciones
- RecepcionHistorialViewModel: Historial de recepciones
- ExistenciaViewModel: Consultar existencias
- LoteInfoViewModel: Información de lotes
- TrasladoCrearViewModel: Creación de traslados
- TrasladoDespachoViewModel: Despacho de traslados
- TrasladoRecepcionViewModel: Recepción de traslados
- TrasladoHistorialViewModel: Historial de traslados
- AlertasCaducidadViewModel: Alertas de caducidad
- AlertasStockViewModel: Alertas de stock mínimo
- FEFOViewModel: Control de lotes FEFO
- BitacoraViewModel: Bitácora de operaciones
- UsuariosViewModel: Gestión de usuarios
- LoginViewModel: Autenticación de usuarios

**Responsabilidades:**
- Interfaz de usuario con WPF
- Patrón MVVM para separación de lógica y vista
- Validaciones de entrada de usuario
- Navegación entre vistas
- Comunicación con servicios de aplicación

---

### 2.5 Altamira.Bodega.App (Capa de Aplicación)

Punto de entrada de la aplicación WPF. Configura Serilog para logging y configura los servicios de dependencias.

**Contenido:**
- App.xaml / App.xaml.cs: Punto de entrada de la aplicación
- Program.cs: Punto de entrada principal con configuración de DI y servicios
- appsettings.json: Configuración de la aplicación

**Responsabilidades:**
- Punto de entrada de la aplicación
- Configuración de inyección de dependencias
- Configuración de logging (Serilog)
- Configuración de aplicación (appsettings.json)

---

## 3. Flujo de Dependencias

```
Presentation → Application → Domain
      ↓
Infrastructure ← Domain
```

La capa de Presentation depende de Application y Domain.
La capa de Infrastructure depende de Domain y Application.
La capa de Application depende de Domain.
La capa de Domain no depende de ninguna otra capa.

---

## 4. Diagramas de Flujo

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

## 5. Patrones de Diseño Utilizados

| Patrón | Descripción | Ubicación |
|--------|-------------|-----------|
| MVVM | Modelo-Vista-ViewModel para separación de lógica y vista | Presentation |
| Dependency Injection | Inyección de dependencias para desacoplamiento | Todos los proyectos |
| Repository Pattern | Acceso a datos a través de clientes API | Infrastructure |
| DTO Pattern | Transferencia de datos entre capas | Domain |
| Observable Pattern | Notificación de cambios en propiedades | Presentation (CommunityToolkit.Mvvm) |
| Singleton | Servicios de token y sesión | Infrastructure |
| Factory Pattern | Creación de clientes API con configuración común | Infrastructure |

---

## 6. Dependencias Externas

| Paquete | Versión | Uso | Capa |
|---------|---------|-----|------|
| CommunityToolkit.Mvvm | 8.4.0 | MVVM, RelayCommand, ObservableProperty | Presentation |
| Microsoft.Extensions.Hosting | 10.0.0 | Inyección de dependencias | Presentation, Infrastructure |
| Microsoft.Extensions.Configuration | 10.0.0 | Configuración | Presentation, Infrastructure |
| Microsoft.Extensions.Configuration.Json | 10.0.0 | Lectura de appsettings.json | Presentation, Infrastructure |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 10.0.0 | Lectura de variables de entorno | Presentation, Infrastructure |
| Serilog | 4.2.0 | Logging | Presentation, Infrastructure |
| Serilog.Sinks.Console | 4.2.0 | Logging a consola | Infrastructure |
| Serilog.Sinks.File | 4.2.0 | Logging a archivo | Infrastructure |
| Serilog.Extensions.Hosting | 9.0.0 | Integración con Hosting | Infrastructure |
| Moq | 4.20.72 | Pruebas unitarias | Tests |
| FluentAssertions | 8.4.0 | Asserciones en pruebas | Tests |
| xUnit | 2.9.3 | Framework de pruebas | Tests |

---

## 7. Estructura de Archivos por Capa

### Domain
```
Altamira.Bodega.Domain/
├── Dtos/
│   ├── ProductoDto.cs
│   ├── ExistenciaDto.cs
│   ├── LoteDto.cs
│   ├── TrasladoDto.cs
│   ├── AlertaDto.cs
│   ├── RecepcionDto.cs
│   ├── AjusteDto.cs
│   ├── UsuarioDto.cs
│   ├── LoginResponse.cs
│   ├── LoginRequest.cs
│   ├── RecepcionCreateDto.cs
│   ├── DetalleRecepcionCreateDto.cs
│   ├── AjusteCreateDto.cs
│   ├── TrasladoCreateDto.cs
│   ├── DetalleRecepcionCreateDto.cs
│   ├── DetalleTrasladoCreateDto.cs
│   ├── TrasladoRecibirDto.cs
│   ├── DetalleRecibirDto.cs
│   ├── ProductoCreateDto.cs
│   ├── ProductoUpdateDto.cs
│   ├── RolDto.cs
│   ├── PermisoDto.cs
│   ├── BitacoraDto.cs
│   ├── ParametroDto.cs
│   ├── UsuarioCreateDto.cs
│   └── UsuarioUpdateDto.cs
├── Interfaces/
│   ├── IInventarioService.cs
│   ├── IProductoService.cs
│   ├── IAuthService.cs
│   ├── IAlertaService.cs
│   ├── ITokenService.cs
│   ├── IRecepcionService.cs
│   ├── IAjusteService.cs
│   ├── ITrasladoService.cs
│   ├── INavigationService.cs
│   └── DomainExceptions.cs
└── Exceptions/
    ├── ReglaNegocioException.cs
    ├── NoEncontradoException.cs
    ├── DatosInvalidosException.cs
    ├── SesionExpiradaException.cs
    ├── SinPermisosException.cs
    ├── ErrorInternoException.cs
    └── ConflictoConcurrenciaException.cs
```

### Application
```
Altamira.Bodega.Application/
├── Interfaces/
│   ├── IInventarioAppService.cs
│   ├── IProductoAppService.cs
│   ├── IAuthAppService.cs
│   ├── IAlertaAppService.cs
│   ├── ISesionAppService.cs
│   ├── IRolAppService.cs
│   └── IBitacoraAppService.cs
└── ApplicationDtos.cs
```

### Infrastructure
```
Altamira.Bodega.Infrastructure/
├── Clients/
│   ├── AuthApiClient.cs
│   ├── InventarioApiClient.cs
│   ├── ProductoApiClient.cs
│   ├── TrasladoApiClient.cs
│   ├── RecepcionApiClient.cs
│   ├── AjusteApiClient.cs
│   ├── AlertaApiClient.cs
│   ├── UsuarioApiClient.cs
│   ├── RolApiClient.cs
│   ├── ParametroApiClient.cs
│   └── BitacoraApiClient.cs
├── Services/
│   ├── TokenService.cs
│   ├── SesionService.cs
│   ├── ApiExceptionMapper.cs
│   └── ApplicationServices.cs (implementación de servicios)
├── DependencyInjection/
│   └── InfrastructureDependencyInjection.cs
└── Clients/
    └── BaseApiClient.cs
```

### Presentation
```
Altamira.Bodega.Presentation/
├── ViewModels/
│   ├── ShellViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── ExistenciaViewModel.cs
│   ├── ProductoViewModel.cs
│   ├── LoteInfoViewModel.cs
│   ├── RecepcionViewModel.cs
│   ├── RecepcionHistorialViewModel.cs
│   ├── ExistenciaViewModel.cs
│   ├── AjustesViewModel.cs
│   ├── TrasladoCrearViewModel.cs
│   ├── TrasladoDespachoViewModel.cs
│   ├── TrasladoRecepcionViewModel.cs
│   ├── TrasladoHistorialViewModel.cs
│   ├── AlertasCaducidadViewModel.cs
│   ├── AlertasStockViewModel.cs
│   ├── FEFOViewModel.cs
│   ├── BitacoraViewModel.cs
│   ├── UsuariosViewModel.cs
│   └── LoginViewModel.cs
├── Views/
│   ├── ShellView.xaml
│   ├── DashboardView.xaml
│   ├── ExistenciasView.xaml
│   ├── CatalogoView.xaml
│   ├── LoteInfoView.xaml
│   ├── RecepcionView.xaml
│   ├── RecepcionHistorialView.xaml
│   ├── AlertasCaducidadView.xaml
│   ├── AlertasStockView.xaml
│   ├── BitacoraView.xaml
│   ├── UsuariosView.xaml
│   ├── LoteInfoView.xaml
│   ├── TrasladoCrearView.xaml
│   ├── TrasladoDespachoView.xaml
│   ├── TrasladoRecepcionView.xaml
│   ├── TrasladoHistorialView.xaml
│   ├── KardexView.xaml
│   └── AjustesView.xaml
├── Resources/
│   ├── Colores.xaml
│   └── Styles.xaml
├── Services/
│   └── NavigationService.cs
├── DependencyInjection/
│   └── PresentationDependencyInjection.cs
└── Views/
    └── VistasCodeBehind.cs
```

### App
```
Altamira.Bodega.App/
├── App.xaml
├── App.xaml.cs
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
└── appsettings.Pruebas.json
```

---

## 8. Flujo de Datos entre Capas

```
Usuario → Vista (XAML) → ViewModel → Servicio de Aplicación → Cliente API → API Central
                                               ↓
                                    Resultado → ViewModel → Vista (muestra datos)
```

Cada capa tiene responsabilidades definidas:
- **View**: Muestra datos y captura entrada de usuario
- **ViewModel**: Contiene lógica de presentación, valida entrada, llama a servicios
- **Servicio de Aplicación**: Orquesta lógica de negocio, valida reglas
- **Cliente API**: Realiza llamadas HTTP a la API central
- **API Central**: Procesa lógica de negocio, accede a base de datos

---

## 9. Consideraciones de Seguridad

- Autenticación JWT contra API central
- Tokens de acceso y refresh almacenados en memoria (TokenService)
- Sesión de usuario en memoria (SesionService)
- Validación de permisos en cliente y servidor
- No se almacenan contraseñas en texto plano
- Los secretos no se incluyen en el instalador

---

## 10. Consideraciones de Rendimiento

- Carga de datos bajo demanda (lazy loading)
- Paginación en consultas de gran volumen
- Uso de ObservableCollection para notificación eficiente de cambios
- Carga asíncrona de datos para no bloquear la interfaz
- Serialización eficiente con System.Text.Json

---

## 11. Consideraciones de Seguridad

- No almacenar contraseñas en texto plano
- No incluir secrets en el código fuente
- Usar variables de entorno para configuración sensible
- Rotar claves de JWT periódicamente
- Validar entradas de usuario en cliente y servidor

---

## 📊 Matriz de Permisos

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

## 📋 Resumen de Capas y Responsabilidades

| Capa | Responsabilidad Principal | Tecnologías Clave |
|------|--------------------------|-------------------|
| Domain | Entidades, DTOs, interfaces de dominio | C#, .NET 10 |
| Application | Contratos de servicios de aplicación | C#, .NET 10 |
| Infrastructure | Clientes API, servicios app, DI | C#, .NET 10, System.Text.Json, Serilog |
| Presentation | Vistas WPF, ViewModels, navegación | WPF, CommunityToolkit.Mvvm, XAML |
| App | Punto de entrada, configuración | WPF, .NET 10, Serilog |

---

## 📞 Soporte

Para soporte técnico de arquitectura, contactar al departamento de arquitectura de Fármacos Altamira.
