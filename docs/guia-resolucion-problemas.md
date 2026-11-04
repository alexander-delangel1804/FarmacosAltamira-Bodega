# Guía de Resolución de Problemas - Módulo de Bodega Fármacos Altamira

## 1. Introducción

Esta guía proporciona soluciones a los problemas más comunes que pueden surgir al usar el Módulo de Bodega de Fármacos Altamira.

---

## 2. Problemas de Inicio de Sesión

### Problema: "Credenciales inválidas o estación no autorizada"

**Síntomas:**
- Al intentar iniciar sesión, aparece el mensaje: "Credenciales inválidas o estación no autorizada."

**Posibles causas:**
1. Nombre de usuario o contraseña incorrectos.
2. El usuario no tiene permisos para acceder al módulo de Bodega.
3. La estación de trabajo no está autorizada.

**Solución:**
1. Verificar que el nombre de usuario y contraseña sean correctos.
2. Verificar que el usuario tenga un rol asignado en el sistema.
3. Contactar al administrador del sistema para verificar los permisos del usuario.
4. Verificar que la estación de trabajo esté registrada en el sistema.

---

### Problema: "No tiene permisos para acceder al sistema"

**Síntomas:**
- Al intentar iniciar sesión, aparece el mensaje: "No tiene permisos para acceder al sistema."

**Posibles causas:**
1. El usuario no tiene ningún rol asignado.
2. El rol del usuario no tiene permisos para el módulo de Bodega.
3. El usuario está bloqueado o inactivo.

**Solución:**
1. Contactar al administrador para verificar los roles y permisos del usuario.
2. Verificar que el usuario esté activo y no bloqueado.
3. Asignar un rol con permisos para el módulo de Bodega si es necesario.

---

### Problema: "Error de conexión"

**Síntomas:**
- Al intentar iniciar sesión o realizar cualquier operación, aparece el mensaje: "Error de conexión".

**Posibles causas:**
1. No hay conexión a internet.
2. La API central no está disponible.
   - URL incorrecta en appsettings.json
   - El servidor de la API está caído
   - Problemas de red

**Solución:**
1. Verificar la conexión a internet.
2. Verificar que la URL de la API en appsettings.json sea correcta.
3. Verificar que el servidor de la API esté en línea.
4. Revisar el archivo de log para más detalles del error.

---

## 2. Problemas de Navegación y Vistas

### Problema: "Vista no registrada: [Nombre de Vista]"

**Síntomas:**
- Al hacer clic en un botón de navegación, aparece un error: "Vista no registrada: [Nombre de Vista]".

**Posibles causas:**
1. El ViewModel no tiene registrado el mapeo a la Vista correspondiente en PresentationDependencyInjection.
2. El archivo XAML de la vista no existe o tiene un nombre incorrecto.
3. Error en el nombre de la clase de la vista.

**Solución:**
1. Verificar que PresentationDependencyInjection.AddPresentation tenga el registro correspondiente:
   ```csharp
   nav.Register<NombreViewModel, NombreView>();
   ```
2. Verificar que el archivo XAML de la vista exista en la carpeta Views.
3. Verificar que el nombre de la clase en el código detrás (.xaml.cs) coincida con el nombre en el XAML.

---

## 3. Problemas de Rendimiento

### Problema: La aplicación es lenta al cargar

**Posibles causas:**
1. Demasiados datos cargados al inicio.
2. Problema de conexión con la API.
3. Recursos insuficientes en la máquina.

**Solución:**
1. Verificar que los datos se carguen bajo demanda (no al inicio).
2. Verificar la velocidad de conexión a la API.
3. Cerrar otras aplicaciones que consuman muchos recursos.
4. Reiniciar la aplicación.

---

### Problema: La aplicación se congela al cargar datos

**Posibles causas:**
1. Consulta de demasiados datos sin paginación.
2. Operación síncrona bloqueando la interfaz de usuario.
3. Problema en el servidor de la API.

**Solución:**
1. Verificar que las consultas usen paginación.
2. Verificar que las operaciones de lectura/escritura sean asíncronas.
3. Verificar el estado de la API central.

---

## 4. Problemas de Datos y Sincronización

### Problema: Los datos no se actualizan

**Posibles causas:**
1. Caché de la aplicación.
2. Problema de conexión con la API.
3. Error en la operación de escritura.

**Solución:**
1. Refrescar la vista (botón Buscar o Refrescar).
2. Verificar conexión a la API.
3. Revisar el mensaje de error si se mostró alguno.

---

### Problema: No se pueden registrar existencias o ajustes

**Posibles causas:**
1. No hay existencias disponibles para el lote seleccionado.
2. La cantidad excede la disponible.
3. El lote está caducado.
4. El producto o lote no está seleccionado.
5. La cantidad es cero o negativa.

**Solución:**
1. Verificar que exista cantidad disponible para el lote.
2. Verificar que la cantidad sea mayor a cero y no exceda la disponible.
3. Verificar que el lote no esté caducado (o tener autorización especial).
4. Seleccionar producto y lote válidos antes de registrar.

---

### Problema: No se puede registrar un traslado

**Posibles causas:**
1. Origen y destino son la misma ubicación.
2. No hay partidas agregadas.
3. No hay existencias disponibles para los lotes seleccionados.

**Solución:**
1. Seleccionar ubicaciones de origen y destino diferentes.
2. Agregar al menos una partida al traslado.
3. Verificar que haya existencias disponibles para los lotes seleccionados.

---

## 5. Problemas de Interfaz de Usuario

### Problema: Los datos no se actualizan después de una operación

**Posibles causas:**
1. La vista no se actualiza automáticamente después de la operación.
2. Caché de datos en el ViewModel.
3. Error en la operación que no se mostró.

**Solución:**
1. Hacer clic en "Buscar" o "Refrescar" para recargar los datos.
2. Verificar si apareció un mensaje de error.
3. Verificar que la operación se completó exitosamente en el servidor.

---

### Problema: Los campos no se actualizan al seleccionar un producto o lote

**Posibles causas:**
1. Error en el binding de datos.
2. Error en la carga de datos dependientes.

**Solución:**
1. Volver a seleccionar el producto o lote.
2. Verificar que los datos estén disponibles en el servidor.
3. Reiniciar la aplicación si el problema persiste.

---

## 6. Problemas de Instalación y Ejecución

### Problema: La aplicación no se inicia

**Posibles causas:**
1. No está instalado .NET 10 Runtime.
2. El archivo ejecutable está dañado.
3. Falta alguna dependencia.

**Solución:**
1. Instalar .NET 10 Runtime desde el sitio oficial de Microsoft.
2. Reinstalar la aplicación.
3. Verificar que todos los archivos del instalador estén presentes.

### Problema: Error al iniciar la aplicación

**Posibles causas:**
1. Archivo appsettings.json dañado o faltante.
2. Configuración de API incorrecta.
3. Conflicto con otra aplicación.

**Solución:**
1. Verificar que appsettings.json exista y tenga el formato correcto.
2. Verificar que la URL de la API sea correcta.
3. Reiniciar la computadora y volver a intentar.

---

## 7. Problemas de Impresión y Reportes

### Problema: No se pueden imprimir reportes

**Posibles causas:**
1. No hay impresora configurada.
2. Problema con el controlador de impresora.
3. La aplicación no tiene implementada la función de impresión.

**Solución:**
1. Verificar que haya una impresora instalada y funcionando.
2. Verificar que el controlador de impresora esté actualizado.
3. Nota: La versión actual puede no tener funcionalidad de impresión implementada.

---

## 7. Problemas de Rendimiento

### Problema: La aplicación es lenta al iniciar

**Posibles causas:**
1. Demasiados datos cargados al inicio.
2. Equipo con pocos recursos.
3. Primera ejecución (carga de ensamblados).

**Solución:**
1. Cerrar otras aplicaciones.
2. Reiniciar la aplicación.
3. Verificar que el equipo cumpla con los requisitos mínimos.

---

## 8. Cómo Reportar un Problema

Si ninguno de los pasos anteriores resuelve el problema, contactar al departamento de sistemas con la siguiente información:

1. **Descripción del problema:** Qué estaba haciendo cuando ocurrió el error.
2. **Mensaje de error:** El texto exacto del mensaje de error.
3. **Pasos para reproducir:** Pasos exactos para reproducir el problema.
4. **Captura de pantalla:** Si es posible, adjuntar una captura de pantalla.
5. **Archivo de log:** El archivo más reciente en `logs/bodega-fecha.txt`.
6. **Información del sistema:** Versión de Windows, versión de .NET, etc.

---

## 7. Ubicación de Archivos de Log

Los archivos de log se encuentran en:
```
C:\Users\[Usuario]\OneDrive\Documentos\Proyecto GPS\Bodega\src\Altamira.Bodega.App\bin\Debug\net10.0-windows\logs\
```

O en la carpeta de instalación si se instaló como aplicación completa.

Cada archivo de log tiene el nombre `bodega-YYYYMMDD.txt` y contiene registros de errores y eventos importantes.

---

## 8. Información para Soporte

Al contactar con soporte, proporcionar:
1. Versión de la aplicación (versión del ensamblado).
2. Versión de Windows y .NET.
3. Descripción detallada del problema.
4. Pasos para reproducir el problema.
5. Capturas de pantalla si es posible.
6. Archivos de log relevantes.

---

## 8. Contacto de Soporte

Para soporte técnico, contactar al departamento de sistemas internos de Fármacos Altamira.

---

## 9. Historial de Cambios

| Versión | Fecha | Descripción |
|---------|-------|-------------|
| 1.0.0 | 2026-11-02 | Versión inicial del Módulo de Bodega |

---

## 10. Apéndice: Códigos de Error Comunes

| Código | Descripción | Solución |
|--------|-------------|----------|
| 400 | Solicitud incorrecta | Verificar datos de entrada |
| 401 | No autorizado | Iniciar sesión nuevamente |
| 403 | Prohibido | No tiene permisos para esta operación |
| 404 | No encontrado | El recurso no existe |
| 500 | Error interno del servidor | Contactar con soporte |
| 503 | Servicio no disponible | La API no está disponible, intentar más tarde |

---
