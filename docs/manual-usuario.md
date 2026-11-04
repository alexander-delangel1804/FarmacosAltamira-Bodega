# Manual de Usuario - Módulo de Bodega Fármacos Altamira

## 1. Introducción

El Módulo de Bodega de Fármacos Altamira es una aplicación de escritorio diseñada para la gestión completa del inventario de la bodega central. Permite gestionar productos, existencias, recepciones, ajustes, traslados, alertas de caducidad y stock mínimo, con un historial completo de movimientos.

## 2. Inicio de Sesión

### Pantalla de Login
Al iniciar la aplicación, se presenta la pantalla de inicio de sesión con los siguientes campos:
- **Usuario**: Nombre de usuario asignado
- **Contraseña**: Contraseña de acceso
- **Estación**: Identificador de la estación de trabajo (predeterminado: 1)

### Proceso de Inicio de Sesión
1. Ingresar nombre de usuario
2. Ingresar contraseña
3. Verificar estación de trabajo
4. Hacer clic en "Iniciar Sesión"
5. Si las credenciales son correctas, se abre el Dashboard principal

### Mensajes de Error
- "El nombre de usuario es obligatorio"
- "La contraseña es obligatoria"
- "Credenciales inválidas o estación no autorizada"
- "No tiene permisos para acceder al sistema"
- "Error de conexión" (si hay problemas de red)

---

## 2. Dashboard (Panel Principal)

El Dashboard muestra tarjetas informativas con los principales indicadores:
- Total de productos en catálogo
- Unidades en bodega
- Productos próximos a caducar
- Productos caducados
- Productos bajo stock mínimo
- Recepciones recientes
- Traslados pendientes
- Movimientos recientes
- Alertas pendientes

### Tarjetas de Información
Cada tarjeta muestra el nombre del indicador, el valor actual y una descripción breve.

---

## 3. Módulos de la Aplicación

### 3.1 Catálogo de Productos
Permite administrar el catálogo de productos con:
- Búsqueda por nombre, código de barras o sustancia activa
- Filtro por productos activos/inactivos
- Paginación para navegar entre páginas
- Información: código de barras, nombre comercial, sustancia activa, presentación, precio, stock mínimo, estado

### 3.2 Existencias por Ubicación
Permite consultar el inventario por ubicación con:
- Filtros por producto, lote y ubicación
- Paginación automática
- Información: producto, lote, fecha de caducidad, ubicación, cantidad, disponible
- Estado de cada existencia

### 3.3 Lotes y Caducidades (Información de Lote)
Muestra información detallada de un lote específico:
- Producto, código de barras
- Número de lote, fecha de caducidad, fecha de ingreso
- Estado del lote
- Cantidad disponible
- Valor del inventario

### 3.4 Kárdex y Movimientos
Muestra el historial de movimientos de inventario con:
- Fecha y hora, producto, lote, ubicación
- Tipo de movimiento (entrada, salida, ajuste)
- Cantidad de entrada/salida
- Motivo, usuario, folio de referencia
- Filtros por producto, lote, ubicación, fecha y tipo

### 3.5 Ajustes de Inventario
Permite registrar ajustes de inventario con:
- Selección de ubicación, producto y lote
- Tipo de ajuste: Positivo (+) o Negativo (-)
- Captura de cantidad y motivo
- Validación de existencia disponible
- Validación de no generar existencias negativas
- Resultado esperado antes de confirmar

### 3.6 Recepción de Mercancía
Permite registrar recepciones de mercancía con:
- Selección de ubicación de destino
- Agregar múltiples partidas (producto, lote, cantidad, caducidad)
- Validación de número de lote
- Validación de cantidad disponible
- Validación de lotes caducados (requiere autorización especial)
- Documento de referencia y observaciones
- Resumen de partidas antes de confirmar

### 3.7 Traslados
Permite gestionar traslados entre ubicaciones con:
- Solicitud de traslado entre ubicaciones
- Despacho de traslados pendientes
- Recepción de traslados entrantes
- Historial completo de traslados
- Estados: SOLICITADO → EN_TRANSITO → RECIBIDO

### 3.8 Alertas de Caducidad
Muestra alertas de lotes por caducidad con:
- Lotes caducados (fecha ya pasada)
- Lotes próximos a caducar (dentro de 30 días)
- Filtros por producto, lote, ubicación y fechas
- Tabla con detalles de cada alerta

### 3.9 Alertas de Stock Mínimo
Muestra alertas de productos por debajo del stock mínimo con:
- Producto, ubicación, cantidad disponible
- Tipo de alerta, mensaje, usuario
- Filtros por producto, ubicación, fechas

### 3.10 Control FEFO
Control de lotes por fecha de caducidad (First Expired, First Out) con:
- Priorización por fecha de caducidad más próxima
- Identificación de lotes elegibles para salida
- Alertas para lotes caducados o próximos a caducar
- Ordenamiento por prioridad (menor días = mayor prioridad)

### 3.11 Bitácora de Operaciones
Muestra el historial de operaciones con:
- Fecha y hora, producto, lote, ubicación
- Tipo de movimiento, cantidad, motivo
- Usuario responsable, folio de referencia
- Filtros por producto, lote, ubicación, fecha, tipo

### 3.12 Usuarios y Permisos
Permite gestionar usuarios y permisos (solo administradores):
- Lista de usuarios con roles y ubicaciones
- Información de estado (activo/bloqueado)
- Intentos fallidos de inicio de sesión

---

## 3. Flujo de Trabajo Típico

### Recepción de Mercancía
1. Navegar a "Recepción de Mercancía"
2. Seleccionar ubicación de destino
3. Agregar partidas: seleccionar producto, lote, cantidad y fecha de caducidad
4. Agregar documento de referencia y observaciones (opcional)
5. Revisar el resumen de partidas
6. Hacer clic en "Registrar Recepción"

### Ajuste de Inventario
1. Navegar a "Ajustes de Inventario"
2. Seleccionar ubicación, producto y lote
3. Seleccionar tipo de ajuste (Positivo o Negativo)
4. Capturar cantidad y motivo
5. Verificar el resultado esperado
6. Hacer clic en "Registrar Ajuste"

### Traslado de Mercancía
1. Navegar a "Traslados" > "Crear Traslado"
2. Seleccionar ubicación de origen y destino
3. Agregar partidas con producto, lote y cantidad
4. Agregar observaciones (opcional)
5. Hacer clic en "Solicitar Traslado"

### Despacho de Traslado
1. Navegar a "Traslados" > "Despacho"
2. Seleccionar la recepción a despachar
3. Hacer clic en "Despachar"

### Recepción de Traslado
1. Navegar a "Traslados" > "Recepción"
2. Seleccionar el traslado a recibir
3. Hacer clic en "Recibir"

---

## 4. Mensajes y Notificaciones

La aplicación muestra mensajes de diferentes tipos:

| Tipo | Color | Significado |
|------|-------|-------------|
| Información | Azul | Información general |
| Éxito | Verde | Operación exitosa |
| Advertencia | Amarillo | Advertencia que requiere atención |
| Error | Rojo | Error que impide la operación |

---

## 5. Buenas Prácticas

### Para Recepción de Mercancía
- Verificar siempre el número de lote antes de agregar
- Verificar la fecha de caducidad antes de agregar
- Verificar la cantidad disponible antes de registrar
- Revisar el resumen antes de confirmar la recepción

### Para Ajustes de Inventario
- Verificar que el lote exista y tenga disponibilidad
- Verificar que la cantidad no exceda la disponible para ajustes negativos
- Proporcionar un motivo claro y descriptivo

### Para Traslados
- Verificar que el origen y destino sean diferentes
- Verificar que haya existencia disponible en el origen
- Verificar los detalles antes de solicitar el traslado

### Para Consultas
- Usar filtros para reducir la cantidad de datos mostrados
- Usar paginación para navegar entre páginas grandes de datos
- Usar búsqueda para encontrar rápidamente productos o lotes específicos

---

## 6. Solución de Problemas Comunes

| Problema | Posible Causa | Solución |
|----------|---------------|----------|
| No se puede iniciar sesión | Credenciales incorrectas | Verificar usuario y contraseña |
| No se pueden cargar existencias | Sin conexión a la API | Verificar conexión a internet |
| No se puede registrar recepción | Sin existencias disponibles | Verificar existencia del lote |
| No se puede registrar ajuste | Existencias insuficientes | Verificar cantidad disponible |
| No se puede registrar traslado | Origen y destino iguales | Seleccionar ubicaciones diferentes |
| Error al cargar vista | Problema de conexión | Verificar conexión a la API |

---

## 7. Soporte Técnico

Para soporte técnico, contactar al departamento de sistemas internos de Fármacos Altamira.
