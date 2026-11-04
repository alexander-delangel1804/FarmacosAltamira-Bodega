# Manual Técnico - Módulo de Bodega Fármacos Altamira

## 1. Arquitectura de la Solución

La aplicación sigue una arquitectura multicapa con separación de responsabilidades claras:

### Capas del Sistema

```
Altamira.Bodega.sln
├── Altamira.Bodega.Domain          (Capa de Dominio)
├── Altamira.Bodega.Application     (Capa de Aplicación)
├── Altamira.Bodega.Infrastructure  (Capa de Infraestructura)
├── Altamira.Bodega.Presentation    (Capa de Presentación WPF)
└── Altamira.Bodega.App             (Capa de Aplicación / Punto de entrada)
```

### Domain Layer
Contiene las entidades de dominio, DTOs (Data Transfer Objects) e interfaces de servicios de dominio. No tiene dependencias externas.

### Application Layer
Contiene las interfaces de servicios de aplicación que definen los contratos de negocio.

### Infrastructure Layer
Implementa los clientes REST para comunicación con la API central, implementación de servicios de aplicación y configuración de inyección de dependencias.

### Presentation Layer
Implementa la interfaz de usuario usando WPF (Windows Presentation Foundation) con el patrón MVVM (Model-View-ViewModel).

### App Layer
Punto de entrada de la aplicación, configura Serilog para logging y configura los servicios de dependencias.

---

## 2. Patrones de Diseño Utilizados

| Patrón | Descripción | Ubicación |
|--------|-------------|-----------|
| MVVM | Modelo-Vista-ViewModel para separación de lógica y vista | Presentation |
| Dependency Injection | Inyección de dependencias para desacoplamiento | Todos los proyectos |
| Repository Pattern | Acceso a datos a través de clientes API | Infrastructure |
| DTO Pattern | Transferencia de datos entre capas | Domain |
| Observable Pattern | Notificación de cambios en propiedades | Presentation (CommunityToolkit.Mvvm) |
| Singleton | Servicios de token y sesión | Infrastructure |

---

## 3. Flujo de Datos

```
Usuario → Vista (XAML) → ViewModel → Servicio de Aplicación → Cliente API → API Central
```

Cada capa tiene responsabilidades definidas:
- **View**: Muestra datos y captura entrada de usuario
- **ViewModel**: Contiene lógica de presentación, valida entrada, llama a servicios
- **Servicio de Aplicación**: Orquesta lógica de negocio, valida reglas
- **Cliente API**: Realiza llamadas HTTP a la API central
- **API Central**: Procesa lógica de negocio, accede a base de datos

---

## 4. Gestión de Estado

La aplicación utiliza CommunityToolkit.Mvvm para implementar [ObservableProperty], lo que permite notificación automática de cambios en propiedades vinculadas a la vista.

Ejemplo:
```csharp
[ObservableProperty]
private string _nombreUsuario = string.Empty;
```

---

## 5. Manejo de Errores

Todas las operaciones asincrónicas están envueltas en bloques try-catch que capturan excepciones y muestran mensajes al usuario a través de la propiedad `MensajeError`.

Ejemplo de manejo de errores:
```csharp
try
{
    var resultado = await _inventarioService.RegistrarRecepcionAsync(dto, idUsuario, CancellationToken.None);
    MensajeExito = $"Recepción registrada correctamente. Folio: {resultado.IdRecepcion}";
}
catch (Exception ex)
{
    MensajeError = $"Error al registrar recepción: {ex.Message}";
}
```

---

## 6. Navegación

La aplicación utiliza un `INavigationService` que permite navegar entre ViewModels. Cada ViewModel tiene un comando `NavigateTo<TViewModel>()` que cambia la vista activa en el ShellView.

Ejemplo:
```csharp
[RelayCommand]
private void NavigateToDashboard()
{
    TituloPaginaActual = "Dashboard";
    _navigationService.NavigateTo<DashboardViewModel>();
}
```

---

## 7. Validaciones

Todas las entradas de usuario se validan antes de ser enviadas a la API:
- Campos obligatorios (nombre de usuario, contraseña, producto, lote, cantidad)
- Cantidades mayores a cero
- Existencias suficientes para ajustes negativos
- Lotes no caducados (salvo autorización explícita)
- Productos y lotes válidos

---

## 8. Seguridad

- Autenticación JWT contra API central
- Tokens de acceso y refresh almacenados en memoria
- Control de acceso basado en roles
- Validación de permisos en cliente y servidor
- No se almacenan contraseñas en texto plano

---

## 9. Logging

La aplicación utiliza Serilog para registrar:
- Eventos de autenticación (login, logout)
- Operaciones de inventario (recepciones, ajustes, traslados)
- Errores y excepciones
- Información de conexión con la API

Los logs se escriben en consola y en archivo rotativo diario.

---

## 10. Consideraciones de Rendimiento

- Carga de datos bajo demanda (lazy loading)
- Paginación en consultas de gran volumen
- Uso de ObservableCollection para notificación eficiente de cambios
- Carga asíncrona de datos para no bloquear la interfaz
