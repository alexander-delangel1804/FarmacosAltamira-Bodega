# Manual de Instalación - Módulo de Bodega Fármacos Altamira

## 1. Requisitos del Sistema

### Requisitos Mínimos
- Sistema Operativo: Windows 10 (versión 1809 o superior) o Windows 11
- Procesador: Intel Core i3 o equivalente (2.0 GHz o superior)
- Memoria RAM: 4 GB mínimo (8 GB recomendado)
- Espacio en disco: 500 MB disponible
- Conexión a internet: Requerida para comunicación con la API central
- Resolución de pantalla: 1366 x 768 mínimo

### Requisitos de Software
- .NET 10 Runtime (incluido en el instalador si se selecciona)
- Microsoft Edge WebView2 (si se requiere para componentes web)

---

## 2. Instalación

### Opción A: Instalador Automático (Recomendado)

1. Descargar el instalador: `Altamira.Bodega.App.Setup.exe`
2. Ejecutar el instalador como administrador
3. Seguir el asistente de instalación:
   - Aceptar términos de licencia
   - Seleccionar carpeta de instalación (por defecto: `C:\Program Files\FarmacosAltamira\Bodega`)
   - Seleccionar componentes a instalar
   - Configurar ubicación de la API (opcional, se puede cambiar después)
   - Finalizar instalación

4. La aplicación se agregará al menú de Inicio bajo "Fármacos Altamira - Módulo de Bodega"

### Opción B: Instalación Manual

1. Descargar el archivo ZIP del proyecto
2. Extraer el contenido en una carpeta de destino
3. Ejecutar `Altamira.Bodega.App.exe`
4. Configurar la URL de la API en `appsettings.json` si es necesario

---

## 3. Configuración Inicial

### Configuración de la API

El archivo `appsettings.json` contiene la configuración de conexión a la API central:

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

### Variables de Entorno

Se pueden sobrescribir los valores de configuración mediante variables de entorno:
- `Altamira_Bodega__Api__BaseUrl`: URL de la API
- `Altamira_Bodega__Api__TimeoutSegundos`: Tiempo de espera en segundos

Ejemplo en PowerShell:
```powershell
$env:Altamira_Bodega__Api__BaseUrl = "https://api.farmacosaltamira.com"
```

---

## 4. Primera Ejecución

1. Iniciar la aplicación desde el menú de Inicio o acceso directo
2. Aparecerá la pantalla de inicio de sesión
3. Ingresar credenciales proporcionadas por el administrador del sistema
4. Seleccionar la estación de trabajo (si aplica)
5. Hacer clic en "Iniciar Sesión"
6. Si las credenciales son correctas, se abrirá el Dashboard principal

---

## 5. Desinstalación

### Desinstalación desde el Panel de Control
1. Abrir "Panel de Control"
2. Ir a "Programas y Características"
3. Buscar "Altamira.Bodega.App - Módulo de Bodega"
4. Hacer clic derecho y seleccionar "Desinstalar"
5. Seguir el asistente de desinstalación

### Desinstalación Manual
1. Eliminar la carpeta de instalación (por defecto: `C:\Program Files\FarmacosAltamira\Bodega`)
2. Eliminar accesos directos del menú de Inicio y escritorio
3. Eliminar archivos de registro si es necesario

---

## 4. Requisitos de Red

La aplicación requiere acceso a la siguiente URL:
- `https://api.farmacosaltamira.com` (o la URL configurada en appsettings.json)

Puertos requeridos:
- Puerto 443 (HTTPS) para comunicación con la API
- Puerto 80 (HTTP) para redirección (si aplica)

---

## 5. Copia de Seguridad

La aplicación no almacena datos localmente de forma permanente. Toda la información se almacena en la API central. Sin embargo, se recomienda:

- Realizar copias de seguridad de los archivos de log en `logs/`
- Respaldar la configuración en `appsettings.json` si se han realizado personalizaciones

---

## 6. Solución de Problemas de Instalación

| Problema | Solución |
|----------|----------|
| No se puede instalar | Verificar privilegios de administrador |
| No se puede iniciar | Verificar .NET 10 runtime instalado |
| Error de conexión | Verificar URL de la API y conexión a internet |
| Error de certificado | Verificar certificado SSL de la API |

---

## 📞 Soporte

Para soporte técnico de instalación, contactar al departamento de sistemas internos de Fármacos Altamira.
