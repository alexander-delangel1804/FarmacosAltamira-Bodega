# Manual de Configuración - Módulo de Bodega Fármacos Altamira

## 1. Configuración de la API

El archivo `appsettings.json` contiene la configuración principal de conexión a la API central:

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
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs\\bodega-.txt", "rollingInterval": "Day", "retainedFileCountLimit": 7 } }
    ]
  },
  "AllowedHosts": "*"
}
```

### Parámetros de Configuración

| Clave | Descripción | Valor por Defecto | Obligatorio |
|-------|-------------|-------------------|-------------|
| `Api:BaseUrl` | URL base de la API central | `http://localhost:5000` | Sí |
| `Api:TimeoutSegundos` | Tiempo de espera para solicitudes en segundos | `30` | No |
| `Jwt:Issuer` | Emisor del token JWT | `FarmacosAltamira` | No |
| `Jwt:Audience` | Audiencia del token JWT | `FarmacosAltamiraApi` | No |
| `Serilog:MinimumLevel:Default` | Nivel mínimo de logging | `Information` | No |
| `Serilog:WriteTo` | Destinos de logs | Consola y archivo | No |

### Variables de Entorno

Se pueden sobrescribir los valores de configuración mediante variables de entorno:

| Variable de Entorno | Configuración Sobrescrita |
|---------------------|---------------------------|
| `Altamira_Bodega__Api__BaseUrl` | `Api:BaseUrl` |
| `Altamira_Bodega__Api__TimeoutSegundos` | `Api:TimeoutSegundos` |
| `Altamira_Bodega__Jwt__Issuer` | `Jwt:Issuer` |
| `Altamira_Bodega__Jwt__Audience` | `Jwt:Audience` |

Ejemplo en PowerShell:
```powershell
$env:Altamira_Bodega__Api__BaseUrl = "https://api.farmacosaltamira.com"
$env:Altamira_Bodega__Api__TimeoutSegundos = "60"
```

Ejemplo en CMD:
```cmd
set Altamira_Bodega__Api__BaseUrl=https://api.farmacosaltamira.com
```

---

## 2. Configuración de Serilog (Logging)

La aplicación utiliza Serilog para registrar eventos. La configuración se encuentra en la sección `Serilog` de `appsettings.json`.

### Niveles de Log
- **Debug**: Información detallada para depuración
- **Information**: Eventos informativos generales
- **Warning**: Advertencias que no impiden el funcionamiento
- **Error**: Errores que afectan funcionalidades específicas
- **Fatal**: Errores críticos que impiden el funcionamiento

### Destinos de Logs
1. **Consola**: Muestra mensajes en la consola de la aplicación
2. **Archivo**: Escribe a `logs/bodega-{fecha}.txt` con rotación diaria y retención de 7 días

### Ejemplo de Entrada de Log
```
2026-11-02 14:26:32.605 -06:00 [INF] Inicio de sesión exitoso para usuario usuario1, rol ADMIN, ubicación 1
2026-11-02 14:26:35.123 -06:00 [WRN] Credenciales inválidas para usuario usuario2
2026-11-02 14:27:01.890 -06:00 [ERR] Error al cargar existencias: Error de conexión
```

---

## 3. Configuración de Sesión

La aplicación maneja la sesión de usuario mediante JWT (JSON Web Token). La sesión incluye:

- Token de acceso
- Token de refresco
- Fecha de expiración
- Información del usuario (ID, nombre, rol, ubicación)

La sesión se almacena en memoria y se pierde al cerrar la aplicación.

### Comportamiento de Sesión
- Al iniciar sesión, se guarda el token y los datos del usuario
- Al cerrar sesión, se limpia la sesión en memoria
- Si el token expira, se debe iniciar sesión nuevamente
- Si la aplicación se cierra, la sesión se pierde

---

## 4. Configuración de Puertos y Red

La aplicación requiere acceso a la siguiente URL:

| Puerto | Protocolo | Propósito |
|--------|-----------|-----------|
| 443 | HTTPS | Comunicación con API central |
| 80 | HTTP | Redirección (si aplica) |

Configuración del firewall:
- Permitir salida en puerto 443 hacia la URL de la API
- Permitir salida en puerto 80 para redirección

---

## 5. Configuración de Imágenes y Recursos

La aplicación utiliza recursos embebidos para iconos y estilos. Estos están incluidos en el ejecutable y no requieren configuración adicional.

---

## 6. Configuración Avanzada

### Cambiar Nivel de Log
Para cambiar el nivel de log en tiempo de ejecución, modificar `appsettings.json`:

```json
"Serilog": {
    "MinimumLevel": {
        "Default": "Debug",  // Cambiar a Debug para más detalles
        "Override": {
            "Microsoft": "Warning",
            "System": "Warning"
        }
    }
}
```

### Cambiar Ubicación de Logs
Para cambiar la ubicación de los archivos de log:

```json
"WriteTo": [
    { "Name": "File", "Args": { "path": "C:\\Logs\\Bodega\\bodega-.txt", "rollingInterval": "Day", "retainedFileCountLimit": 30 } }
]
```

---

## 6. Resetear Configuración

Para restablecer la configuración a los valores predeterminados:
1. Cerrar la aplicación
2. Eliminar o renombrar el archivo `appsettings.json`
3. Reiniciar la aplicación (se creará uno nuevo con valores predeterminados)

---

## 7. Soporte

Para soporte técnico de configuración, contactar al departamento de sistemas internos.
