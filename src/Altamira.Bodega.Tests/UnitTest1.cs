using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Presentation.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace Altamira.Bodega.Tests;

public class ExistenciaViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly ExistenciaViewModel _viewModel;

    public ExistenciaViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _viewModel = new ExistenciaViewModel(_inventarioServiceMock.Object);
    }

    [Fact]
    public void ExistenciaViewModel_InitialState_ShouldHaveEmptyExistencias()
    {
        _viewModel.Existencias.Should().NotBeNull();
        _viewModel.Existencias.Should().BeEmpty();
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public async Task ExistenciaViewModel_CargarExistencias_ShouldPopulateList()
    {
        var existencias = new List<ExistenciaDto>
        {
            new ExistenciaDto(1, "Aspirina 500mg", "ABC123", 1, "LOTE001", DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 1, "Bodega Central", 100, 0, 100),
            new ExistenciaDto(2, "Paracetamol 500mg", "DEF456", 2, "LOTE002", DateOnly.FromDateTime(DateTime.Now.AddDays(60)), 1, "Bodega Central", 50, 0, 50)
        };

        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existencias);

        await _viewModel.RefrescarAsync();

        _viewModel.Existencias.Should().HaveCount(2);
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public void ExistenciaViewModel_Busqueda_ShouldBeEmptyInitially()
    {
        _viewModel.Busqueda.Should().Be(string.Empty);
    }

    [Fact]
    public void ExistenciaViewModel_FiltroEstado_ShouldBeEmptyInitially()
    {
        _viewModel.FiltroEstado.Should().Be(string.Empty);
    }

    [Fact]
    public void ExistenciaViewModel_PaginaActual_ShouldBeOneInitially()
    {
        _viewModel.PaginaActual.Should().Be(1);
    }

    [Fact]
    public void ExistenciaViewModel_TamanoPagina_ShouldBeFiftyInitially()
    {
        _viewModel.TamanoPagina.Should().Be(50);
    }

    [Fact]
    public async Task ExistenciaViewModel_RefrescarAsync_WhenServiceThrows_ShouldSetMensajeError()
    {
        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(null, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de conexión"));

        await _viewModel.RefrescarAsync();

        _viewModel.MensajeError.Should().NotBeNull();
        _viewModel.EstaCargando.Should().BeFalse();
    }
}

public class RecepcionViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly RecepcionViewModel _viewModel;

    public RecepcionViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(null, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExistenciaDto>());

        _viewModel = new RecepcionViewModel(_inventarioServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void RecepcionViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.Partidas.Should().NotBeNull();
        _viewModel.Partidas.Should().BeEmpty();
        _viewModel.TotalPartidas.Should().Be(0);
        _viewModel.TotalCantidad.Should().Be(0);
        _viewModel.NoHayPartidas.Should().BeTrue();
        _viewModel.HayPartidas.Should().BeFalse();
    }

    [Fact]
    public void RecepcionViewModel_AgregarPartida_WithoutProduct_ShouldSetError()
    {
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;

        _viewModel.AgregarPartida();

        _viewModel.MensajeError.Should().Be("Debe seleccionar un producto válido.");
        _viewModel.TotalPartidas.Should().Be(0);
    }

    [Fact]
    public void RecepcionViewModel_AgregarPartida_WithoutLote_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.Cantidad = 10;

        _viewModel.AgregarPartida();

        _viewModel.MensajeError.Should().Be("El número de lote es obligatorio.");
        _viewModel.TotalPartidas.Should().Be(0);
    }

    [Fact]
    public void RecepcionViewModel_AgregarPartida_WithZeroQuantity_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 0;

        _viewModel.AgregarPartida();

        _viewModel.MensajeError.Should().Be("La cantidad debe ser mayor a cero.");
        _viewModel.TotalPartidas.Should().Be(0);
    }

    [Fact]
    public void RecepcionViewModel_AgregarPartida_WithExpiredLot_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(-1); // Lote caducado
        _viewModel.LoteCaducadoPermitido = false;

        _viewModel.AgregarPartida();

        _viewModel.MensajeError.Should().Contain("caducado");
        _viewModel.TotalPartidas.Should().Be(0);
    }

    [Fact]
    public void RecepcionViewModel_AgregarPartida_Valid_ShouldAddPartida()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(30);

        _viewModel.AgregarPartida();

        _viewModel.MensajeError.Should().BeNull();
        _viewModel.TotalPartidas.Should().Be(1);
        _viewModel.TotalCantidad.Should().Be(10);
        _viewModel.HayPartidas.Should().BeTrue();
        _viewModel.NoHayPartidas.Should().BeFalse();
    }

    [Fact]
    public void RecepcionViewModel_AgregarPartida_DuplicateLote_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(30);

        _viewModel.AgregarPartida();
        _viewModel.TotalPartidas.Should().Be(1);

        // Reestablecer valores para intentar duplicado
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 5;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(30);

        _viewModel.AgregarPartida();

        _viewModel.MensajeError.Should().Contain("Ya existe una partida");
        _viewModel.TotalPartidas.Should().Be(1);
    }

    [Fact]
    public void RecepcionViewModel_QuitarPartida_ShouldRemovePartida()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(30);

        _viewModel.AgregarPartida();
        _viewModel.TotalPartidas.Should().Be(1);

        var partida = _viewModel.Partidas.First();
        _viewModel.QuitarPartida(partida);

        _viewModel.TotalPartidas.Should().Be(0);
        _viewModel.NoHayPartidas.Should().BeTrue();
    }

    [Fact]
    public async Task RecepcionViewModel_RegistrarRecepcion_WithoutPartidas_ShouldSetError()
    {
        _viewModel.IdUbicacion = 1;

        await _viewModel.RegistrarRecepcionAsync();

        _viewModel.MensajeError.Should().Be("Debe agregar al menos una partida.");
    }

    [Fact]
    public async Task RecepcionViewModel_RegistrarRecepcion_Valid_ShouldCallServiceAndClear()
    {
        var recepcionDto = new RecepcionDto(1, 1, "Bodega Central", 1, "usuario", DateTimeOffset.Now, "DOC001", null);

        _inventarioServiceMock
            .Setup(s => s.RegistrarRecepcionAsync(It.IsAny<RecepcionCreateDto>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recepcionDto);

        _viewModel.IdUbicacion = 1;
        _viewModel.DocumentoReferencia = "DOC001";
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(30);

        _viewModel.AgregarPartida();

        await _viewModel.RegistrarRecepcionAsync();

        _viewModel.MensajeExito.Should().Contain("registrada correctamente");
        _viewModel.MensajeError.Should().BeNull();
        _viewModel.TotalPartidas.Should().Be(0);
        _viewModel.NoHayPartidas.Should().BeTrue();

        _inventarioServiceMock.Verify(s => s.RegistrarRecepcionAsync(It.IsAny<RecepcionCreateDto>(), 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void RecepcionViewModel_LimpiarTodo_ShouldResetState()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.NumeroLoteNuevo = "LOTE001";
        _viewModel.Cantidad = 10;
        _viewModel.FechaCaducidad = DateTime.Now.AddDays(30);

        _viewModel.AgregarPartida();
        _viewModel.TotalPartidas.Should().Be(1);

        _viewModel.LimpiarTodo();

        _viewModel.TotalPartidas.Should().Be(0);
        _viewModel.NoHayPartidas.Should().BeTrue();
        _viewModel.MensajeError.Should().BeNull();
        _viewModel.MensajeExito.Should().BeEmpty();
    }
}

public class RecepcionHistorialViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly RecepcionHistorialViewModel _viewModel;

    public RecepcionHistorialViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _viewModel = new RecepcionHistorialViewModel(_inventarioServiceMock.Object);
    }

    [Fact]
    public void RecepcionHistorialViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.Movimientos.Should().NotBeNull();
        _viewModel.Movimientos.Should().BeEmpty();
        _viewModel.TotalMovimientos.Should().Be(0);
        _viewModel.NoHayMovimientos.Should().BeTrue();
    }

    [Fact]
    public async Task RecepcionHistorialViewModel_CargarRecepciones_ShouldPopulateList()
    {
        var movimientos = new List<MovimientoDto>
        {
            new MovimientoDto(1, 1, "Aspirina 500mg", "LOTE001", 1, "Bodega Central", "usuario", DateTimeOffset.Now, "RECEPCION", 100, null, "DOC001", 1),
            new MovimientoDto(2, 2, "Paracetamol 500mg", "LOTE002", 1, "Bodega Central", "usuario", DateTimeOffset.Now.AddDays(-1), "RECEPCION", 50, null, "DOC002", 2)
        };

        _inventarioServiceMock
            .Setup(s => s.ListarRecepcionesAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movimientos);

        await _viewModel.CargarRecepcionesAsync();

        _viewModel.TotalMovimientos.Should().Be(2);
        _viewModel.NoHayMovimientos.Should().BeFalse();
        _viewModel.HayMovimientos.Should().BeTrue();
    }

    [Fact]
    public async Task RecepcionHistorialViewModel_CargarRecepciones_WhenServiceThrows_ShouldSetError()
    {
        _inventarioServiceMock
            .Setup(s => s.ListarRecepcionesAsync(null, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de conexión"));

        await _viewModel.CargarRecepcionesAsync();

        _viewModel.MensajeError.Should().NotBeNull();
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public void RecepcionHistorialViewModel_LimpiarFiltros_ShouldResetFilters()
    {
        _viewModel.Busqueda = "test";
        _viewModel.FechaDesde = DateTimeOffset.Now.AddDays(-30);
        _viewModel.FechaHasta = DateTimeOffset.Now;

        _viewModel.LimpiarFiltros();

        _viewModel.Busqueda.Should().Be(string.Empty);
        _viewModel.FechaDesde.Should().BeNull();
        _viewModel.FechaHasta.Should().BeNull();
    }
}

public class KardexViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly KardexViewModel _viewModel;

    public KardexViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _viewModel = new KardexViewModel(_inventarioServiceMock.Object);
    }

    [Fact]
    public void KardexViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.Movimientos.Should().NotBeNull();
        _viewModel.Movimientos.Should().BeEmpty();
        _viewModel.TotalMovimientos.Should().Be(0);
        _viewModel.NoHayMovimientos.Should().BeTrue();
        _viewModel.PaginaActual.Should().Be(1);
    }

    [Fact]
    public async Task KardexViewModel_CargarMovimientos_ShouldPopulateList()
    {
        var movimientos = new List<MovimientoDto>
        {
            new MovimientoDto(1, 1, "Aspirina 500mg", "LOTE001", 1, "Bodega Central", "usuario1", DateTimeOffset.Now, "ENTRADA", 100, null, "DOC001", 1),
            new MovimientoDto(2, 1, "Paracetamol 500mg", "LOTE002", 1, "Bodega Central", "usuario2", DateTimeOffset.Now.AddHours(-1), "SALIDA", 50, null, "DOC002", 2),
            new MovimientoDto(3, 2, "Ibuprofeno 400mg", "LOTE003", 2, "Bodega Norte", "usuario1", DateTimeOffset.Now.AddHours(-2), "AJUSTE", 10, "Corrección", "DOC003", 3)
        };

        _inventarioServiceMock
            .Setup(s => s.ConsultarMovimientosAsync(null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movimientos);

        await _viewModel.CargarMovimientosAsync();

        _viewModel.TotalMovimientos.Should().Be(3);
        _viewModel.NoHayMovimientos.Should().BeFalse();
        _viewModel.HayMovimientos.Should().BeTrue();
        _viewModel.TotalEntradas.Should().Be(100);
        _viewModel.TotalSalidas.Should().Be(50);
    }

    [Fact]
    public async Task KardexViewModel_CargarMovimientos_WhenServiceThrows_ShouldSetError()
    {
        _inventarioServiceMock
            .Setup(s => s.ConsultarMovimientosAsync(null, null, null, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de conexión"));

        await _viewModel.CargarMovimientosAsync();

        _viewModel.MensajeError.Should().NotBeNull();
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public void KardexViewModel_LimpiarFiltros_ShouldResetFilters()
    {
        _viewModel.Busqueda = "test";
        _viewModel.FiltroTipo = "ENTRADA";
        _viewModel.FiltroProducto = "Aspirina";
        _viewModel.FiltroLote = "LOTE001";
        _viewModel.FiltroUbicacion = 1;
        _viewModel.FechaDesde = DateTimeOffset.Now.AddDays(-30);
        _viewModel.FechaHasta = DateTimeOffset.Now;
        _viewModel.PaginaActual = 3;

        _viewModel.LimpiarFiltros();

        _viewModel.Busqueda.Should().Be(string.Empty);
        _viewModel.FiltroTipo.Should().Be(string.Empty);
        _viewModel.FiltroProducto.Should().Be(string.Empty);
        _viewModel.FiltroLote.Should().Be(string.Empty);
        _viewModel.FiltroUbicacion.Should().BeNull();
        _viewModel.FechaDesde.Should().BeNull();
        _viewModel.FechaHasta.Should().BeNull();
        _viewModel.PaginaActual.Should().Be(1);
    }

    [Fact]
    public void KardexViewModel_CambiarPagina_ShouldUpdatePage()
    {
        _viewModel.TotalPaginas = 5;
        _viewModel.PaginaActual = 1;

        _viewModel.CambiarPaginaCommand.Execute(1);
        _viewModel.PaginaActual.Should().Be(2);

        _viewModel.CambiarPaginaCommand.Execute(-1);
        _viewModel.PaginaActual.Should().Be(1);
    }

    [Fact]
    public void KardexViewModel_CambiarPagina_ShouldNotGoBelowOne()
    {
        _viewModel.TotalPaginas = 5;
        _viewModel.PaginaActual = 1;

        _viewModel.CambiarPaginaCommand.Execute(-1);
        _viewModel.PaginaActual.Should().Be(1);
    }
}

public class AjustesViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly Mock<IProductoAppService> _productoServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly AjustesViewModel _viewModel;

    public AjustesViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _productoServiceMock = new Mock<IProductoAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);
        _sesionServiceMock.Setup(s => s.PuedeAccederAMinistro("INVENTARIO", "AJUSTES")).Returns(true);

        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExistenciaDto>
            {
                new ExistenciaDto(1, "Aspirina 500mg", "ABC123", 1, "LOTE001", DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 1, "Bodega Central", 100, 0, 100)
            });

        _viewModel = new AjustesViewModel(_inventarioServiceMock.Object, _productoServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void AjustesViewModel_InitialState_ShouldHaveDefaults()
    {
        _viewModel.IdUbicacion.Should().Be(1);
        _viewModel.CantidadAjuste.Should().Be(1);
        _viewModel.TipoAjuste.Should().Be(TipoAjuste.Positivo);
        _viewModel.ExistenciaActual.Should().Be(0);
        _viewModel.ResultadoEsperado.Should().Be(0);
        _viewModel.PermiteAjustes.Should().BeTrue();
    }

    [Fact]
    public void AjustesViewModel_CalcularResultado_Positivo_ShouldAdd()
    {
        _viewModel.ExistenciaActual = 100;
        _viewModel.CantidadAjuste = 25;
        _viewModel.TipoAjuste = TipoAjuste.Positivo;

        _viewModel.ResultadoEsperado.Should().Be(125);
    }

    [Fact]
    public void AjustesViewModel_CalcularResultado_Negativo_ShouldSubtract()
    {
        _viewModel.ExistenciaActual = 100;
        _viewModel.CantidadAjuste = 25;
        _viewModel.TipoAjuste = TipoAjuste.Negativo;

        _viewModel.ResultadoEsperado.Should().Be(75);
    }

[Fact]
    public async Task AjustesViewModel_RegistrarAjuste_SinProducto_ShouldSetError()
    {
        await _viewModel.RegistrarAjusteAsync();

        _viewModel.MensajeError.Should().Be("Debe seleccionar un producto.");
    }

    [Fact]
    public async Task AjustesViewModel_RegistrarAjuste_SinLote_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        await _viewModel.RegistrarAjusteAsync();

        _viewModel.MensajeError.Should().Be("Debe seleccionar un lote.");
    }

    [Fact]
    public async Task AjustesViewModel_RegistrarAjuste_CantidadCero_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.CantidadAjuste = 0;

        await _viewModel.RegistrarAjusteAsync();

        _viewModel.MensajeError.Should().Be("La cantidad de ajuste debe ser mayor a cero.");
    }

    [Fact]
    public async Task AjustesViewModel_RegistrarAjuste_SinMotivo_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.CantidadAjuste = 10;
        _viewModel.Motivo = string.Empty;

        await _viewModel.RegistrarAjusteAsync();

        _viewModel.MensajeError.Should().Be("El motivo es obligatorio.");
    }

    [Fact]
    public async Task AjustesViewModel_RegistrarAjuste_NegativoExistenciaInsuficiente_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.ExistenciaActual = 5;
        _viewModel.CantidadAjuste = 10;
        _viewModel.TipoAjuste = TipoAjuste.Negativo;
        _viewModel.Motivo = "Merma";

        await _viewModel.RegistrarAjusteAsync();

        _viewModel.MensajeError.Should().Contain("menor que la cantidad solicitada");
    }

    [Fact]
    public async Task AjustesViewModel_RegistrarAjuste_Valido_ShouldCallService()
    {
        var ajusteDto = new AjusteDto(1, 1, 10, "Corrección de inventario");

        _inventarioServiceMock
            .Setup(s => s.RegistrarAjusteAsync(It.IsAny<AjusteCreateDto>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ajusteDto);

        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.CantidadAjuste = 10;
        _viewModel.TipoAjuste = TipoAjuste.Positivo;
        _viewModel.Motivo = "Corrección de inventario";

        await _viewModel.RegistrarAjusteAsync();

        _viewModel.MensajeExito.Should().Contain("registrado correctamente");
        _viewModel.MensajeError.Should().BeNull();

        _inventarioServiceMock.Verify(s => s.RegistrarAjusteAsync(It.IsAny<AjusteCreateDto>(), 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AjustesViewModel_PermiteAjustes_Falso_ShouldSetError()
    {
        _sesionServiceMock.Setup(s => s.PuedeAccederAMinistro("INVENTARIO", "AJUSTES")).Returns(false);
        var vmSinPermisos = new AjustesViewModel(_inventarioServiceMock.Object, _productoServiceMock.Object, _sesionServiceMock.Object);

        await vmSinPermisos.RegistrarAjusteAsync();

        vmSinPermisos.MensajeError.Should().Be("No tiene permisos para registrar ajustes de inventario.");
    }

    [Fact]
    public void AjustesViewModel_LimpiarTodo_ShouldResetState()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.ExistenciaActual = 100;
        _viewModel.CantidadAjuste = 25;
        _viewModel.TipoAjuste = TipoAjuste.Negativo;
        _viewModel.Motivo = "Prueba";

        _viewModel.LimpiarTodo();

        _viewModel.IdProductoSeleccionado.Should().Be(0);
        _viewModel.NombreProductoSeleccionado.Should().Be(string.Empty);
        _viewModel.IdLoteSeleccionado.Should().Be(0);
        _viewModel.NumeroLoteSeleccionado.Should().Be(string.Empty);
        _viewModel.ExistenciaActual.Should().Be(0);
        _viewModel.CantidadAjuste.Should().Be(1);
        _viewModel.TipoAjuste.Should().Be(TipoAjuste.Positivo);
        _viewModel.Motivo.Should().Be(string.Empty);
        _viewModel.ResultadoEsperado.Should().Be(0);
        _viewModel.MensajeError.Should().BeNull();
        _viewModel.MensajeExito.Should().BeEmpty();
    }
}

public class TrasladoCrearViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly Mock<ITrasladoAppService> _trasladoServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly TrasladoCrearViewModel _viewModel;

    public TrasladoCrearViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _trasladoServiceMock = new Mock<ITrasladoAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExistenciaDto>
            {
                new ExistenciaDto(1, "Aspirina 500mg", "ABC123", 1, "LOTE001", DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 1, "Bodega Central", 100, 0, 100)
            });

        _viewModel = new TrasladoCrearViewModel(_inventarioServiceMock.Object, _trasladoServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void TrasladoCrearViewModel_InitialState_ShouldHaveDefaults()
    {
        _viewModel.IdOrigen.Should().Be(1);
        _viewModel.IdDestino.Should().Be(0);
        _viewModel.Detalles.Should().BeEmpty();
        _viewModel.TotalDetalles.Should().Be(0);
        _viewModel.TotalCantidad.Should().Be(0);
        _viewModel.HayDetalles.Should().BeFalse();
        _viewModel.PuedeAgregarDetalle.Should().BeFalse();
    }

    [Fact]
    public void TrasladoCrearViewModel_AgregarDetalle_SinProducto_ShouldSetError()
    {
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.CantidadEnviar = 10;

        _viewModel.AgregarDetalle();

        _viewModel.MensajeError.Should().Be("Debe seleccionar un producto.");
        _viewModel.TotalDetalles.Should().Be(0);
    }

    [Fact]
    public void TrasladoCrearViewModel_AgregarDetalle_SinLote_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.CantidadEnviar = 10;

        _viewModel.AgregarDetalle();

        _viewModel.MensajeError.Should().Be("Debe seleccionar un lote.");
        _viewModel.TotalDetalles.Should().Be(0);
    }

    [Fact]
    public void TrasladoCrearViewModel_AgregarDetalle_CantidadCero_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.CantidadEnviar = 0;

        _viewModel.AgregarDetalle();

        _viewModel.MensajeError.Should().Be("La cantidad debe ser mayor a cero.");
        _viewModel.TotalDetalles.Should().Be(0);
    }

    [Fact]
    public void TrasladoCrearViewModel_AgregarDetalle_CantidadExcedeExistencia_ShouldSetError()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.ExistenciaDisponible = 5;
        _viewModel.CantidadEnviar = 10;

        _viewModel.AgregarDetalle();

        _viewModel.MensajeError.Should().Contain("excede la existencia disponible");
        _viewModel.TotalDetalles.Should().Be(0);
    }

    [Fact]
    public void TrasladoCrearViewModel_AgregarDetalle_Valido_ShouldAddDetalle()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.ExistenciaDisponible = 100;
        _viewModel.CantidadEnviar = 10;

        _viewModel.AgregarDetalle();

        _viewModel.MensajeError.Should().BeNull();
        _viewModel.TotalDetalles.Should().Be(1);
        _viewModel.TotalCantidad.Should().Be(10);
        _viewModel.HayDetalles.Should().BeTrue();
    }

    [Fact]
    public void TrasladoCrearViewModel_QuitarDetalle_ShouldRemoveDetalle()
    {
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.ExistenciaDisponible = 100;
        _viewModel.CantidadEnviar = 10;

        _viewModel.AgregarDetalle();
        _viewModel.TotalDetalles.Should().Be(1);

        var detalle = _viewModel.Detalles.First();
        _viewModel.QuitarDetalle(detalle);

        _viewModel.TotalDetalles.Should().Be(0);
        _viewModel.HayDetalles.Should().BeFalse();
    }

    [Fact]
    public async Task TrasladoCrearViewModel_SolicitarTraslado_SinDestino_ShouldSetError()
    {
        _viewModel.IdOrigen = 1;

        await _viewModel.SolicitarTrasladoAsync();

        _viewModel.MensajeError.Should().Be("Debe seleccionar una ubicación de destino.");
    }

    [Fact]
    public async Task TrasladoCrearViewModel_SolicitarTraslado_OrigenIgualDestino_ShouldSetError()
    {
        _viewModel.IdOrigen = 1;
        _viewModel.IdDestino = 1;

        await _viewModel.SolicitarTrasladoAsync();

        _viewModel.MensajeError.Should().Be("El origen y destino no pueden ser la misma ubicación.");
    }

    [Fact]
    public async Task TrasladoCrearViewModel_SolicitarTraslado_SinDetalles_ShouldSetError()
    {
        _viewModel.IdOrigen = 1;
        _viewModel.IdDestino = 2;

        await _viewModel.SolicitarTrasladoAsync();

        _viewModel.MensajeError.Should().Be("Debe agregar al menos un detalle al traslado.");
    }

    [Fact]
    public async Task TrasladoCrearViewModel_SolicitarTraslado_Valido_ShouldCallService()
    {
        var trasladoDto = new TrasladoDto(1, "TRF-001", 1, "Bodega Central", 2, "Sucursal Norte", 1, "usuario1", DateTimeOffset.Now, null, null, "SOLICITADO", null);

        _trasladoServiceMock
            .Setup(s => s.SolicitarTrasladoAsync(It.IsAny<TrasladoCreateDto>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trasladoDto);

        _viewModel.IdOrigen = 1;
        _viewModel.IdDestino = 2;
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.ExistenciaDisponible = 100;
        _viewModel.CantidadEnviar = 10;

        _viewModel.AgregarDetalle();

        await _viewModel.SolicitarTrasladoAsync();

        _viewModel.MensajeExito.Should().Contain("solicitado correctamente");
        _viewModel.MensajeError.Should().BeNull();
        _viewModel.TotalDetalles.Should().Be(0);
        _viewModel.HayDetalles.Should().BeFalse();

        _trasladoServiceMock.Verify(s => s.SolicitarTrasladoAsync(It.IsAny<TrasladoCreateDto>(), 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void TrasladoCrearViewModel_LimpiarTodo_ShouldResetState()
    {
        _viewModel.IdDestino = 2;
        _viewModel.Observaciones = "Prueba";
        _viewModel.IdProductoSeleccionado = 1;
        _viewModel.NombreProductoSeleccionado = "Aspirina";
        _viewModel.IdLoteSeleccionado = 1;
        _viewModel.NumeroLoteSeleccionado = "LOTE001";
        _viewModel.ExistenciaDisponible = 100;
        _viewModel.CantidadEnviar = 10;
        _viewModel.MensajeError = "Error previo";
        _viewModel.MensajeExito = "Éxito previo";

        _viewModel.LimpiarTodo();

        _viewModel.IdDestino.Should().Be(0);
        _viewModel.Observaciones.Should().Be(string.Empty);
        _viewModel.IdProductoSeleccionado.Should().Be(0);
        _viewModel.NombreProductoSeleccionado.Should().Be(string.Empty);
        _viewModel.IdLoteSeleccionado.Should().Be(0);
        _viewModel.NumeroLoteSeleccionado.Should().Be(string.Empty);
        _viewModel.ExistenciaDisponible.Should().Be(0);
        _viewModel.CantidadEnviar.Should().Be(1);
        _viewModel.TotalDetalles.Should().Be(0);
        _viewModel.HayDetalles.Should().BeFalse();
        _viewModel.MensajeError.Should().BeNull();
        _viewModel.MensajeExito.Should().BeEmpty();
    }
}

public class TrasladoDespachoViewModelTests
{
    private readonly Mock<ITrasladoAppService> _trasladoServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly TrasladoDespachoViewModel _viewModel;

    public TrasladoDespachoViewModelTests()
    {
        _trasladoServiceMock = new Mock<ITrasladoAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _viewModel = new TrasladoDespachoViewModel(_trasladoServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void TrasladoDespachoViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.TrasladosPendientes.Should().NotBeNull();
        _viewModel.TrasladosPendientes.Should().BeEmpty();
        _viewModel.TotalPendientes.Should().Be(0);
    }

    [Fact]
    public async Task TrasladoDespachoViewModel_CargarTraslados_ShouldPopulateList()
    {
        var traslados = new List<TrasladoDto>
        {
            new TrasladoDto(1, "TRF-001", 1, "Bodega Central", 2, "Sucursal Norte", 1, "usuario1", DateTimeOffset.Now, null, null, "SOLICITADO", null),
            new TrasladoDto(2, "TRF-002", 1, "Bodega Central", 3, "Sucursal Sur", 1, "usuario1", DateTimeOffset.Now.AddHours(-1), null, null, "SOLICITADO", null)
        };

        _trasladoServiceMock
            .Setup(s => s.ListarTrasladosAsync("SOLICITADO", 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(traslados);

        await _viewModel.CargarTrasladosPendientesAsync();

        _viewModel.TotalPendientes.Should().Be(2);
    }

    [Fact]
    public async Task TrasladoDespachoViewModel_DespacharTraslado_ShouldSetSuccess()
    {
        var trasladoDespachado = new TrasladoDto(1, "TRF-001", 1, "Bodega Central", 2, "Sucursal Norte", 1, "usuario1", DateTimeOffset.Now, null, null, "EN_TRANSITO", null);

        _trasladoServiceMock
            .Setup(s => s.DespacharTrasladoAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trasladoDespachado);

        await _viewModel.DespacharTrasladoAsync(1);

        _viewModel.MensajeExito.Should().Contain("despachado correctamente");
    }

    [Fact]
    public async Task TrasladoDespachoViewModel_DespacharTraslado_WhenServiceThrows_ShouldSetError()
    {
        _trasladoServiceMock
            .Setup(s => s.DespacharTrasladoAsync(1, 1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de despacho"));

        await _viewModel.DespacharTrasladoAsync(1);

        _viewModel.MensajeError.Should().NotBeNull();
    }
}

public class TrasladoRecepcionViewModelTests
{
    private readonly Mock<ITrasladoAppService> _trasladoServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly TrasladoRecepcionViewModel _viewModel;

    public TrasladoRecepcionViewModelTests()
    {
        _trasladoServiceMock = new Mock<ITrasladoAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(2);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _viewModel = new TrasladoRecepcionViewModel(_trasladoServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void TrasladoRecepcionViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.TrasladosARecibir.Should().NotBeNull();
        _viewModel.TrasladosARecibir.Should().BeEmpty();
        _viewModel.TotalARecibir.Should().Be(0);
        _viewModel.HayTraslados.Should().BeFalse();
    }

    [Fact]
    public async Task TrasladoRecepcionViewModel_CargarTraslados_ShouldPopulateList()
    {
        var traslados = new List<TrasladoDto>
        {
            new TrasladoDto(1, "TRF-001", 1, "Bodega Central", 2, "Sucursal Norte", 1, "usuario1", DateTimeOffset.Now, null, null, "EN_TRANSITO", null)
        };

        _trasladoServiceMock
            .Setup(s => s.ListarTrasladosAsync("EN_TRANSITO", null, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(traslados);

        await _viewModel.CargarTrasladosARecibirAsync();

        _viewModel.TotalARecibir.Should().Be(1);
        _viewModel.HayTraslados.Should().BeTrue();
    }

    [Fact]
    public async Task TrasladoRecepcionViewModel_RecibirTraslado_ShouldSetSuccess()
    {
        var trasladoRecibido = new TrasladoDto(1, "TRF-001", 1, "Bodega Central", 2, "Sucursal Norte", 1, "usuario1", DateTimeOffset.Now, null, DateTimeOffset.Now, "RECIBIDO", null);

        _trasladoServiceMock
            .Setup(s => s.RecibirTrasladoAsync(1, It.IsAny<TrasladoRecibirDto>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trasladoRecibido);

        await _viewModel.RecibirTrasladoAsync(1);

        _viewModel.MensajeExito.Should().Contain("recibido correctamente");
    }
}

public class TrasladoHistorialViewModelTests
{
    private readonly Mock<ITrasladoAppService> _trasladoServiceMock;
    private readonly TrasladoHistorialViewModel _viewModel;

    public TrasladoHistorialViewModelTests()
    {
        _trasladoServiceMock = new Mock<ITrasladoAppService>();
        _viewModel = new TrasladoHistorialViewModel(_trasladoServiceMock.Object);
    }

    [Fact]
    public void TrasladoHistorialViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.Traslados.Should().NotBeNull();
        _viewModel.Traslados.Should().BeEmpty();
        _viewModel.TotalTraslados.Should().Be(0);
        _viewModel.HayTraslados.Should().BeFalse();
    }

    [Fact]
    public async Task TrasladoHistorialViewModel_CargarTraslados_ShouldPopulateList()
    {
        var traslados = new List<TrasladoDto>
        {
            new TrasladoDto(1, "TRF-001", 1, "Bodega Central", 2, "Sucursal Norte", 1, "usuario1", DateTimeOffset.Now, null, null, "SOLICITADO", null),
            new TrasladoDto(2, "TRF-002", 2, "Sucursal Norte", 3, "Sucursal Sur", 1, "usuario1", DateTimeOffset.Now.AddHours(-1), null, null, "RECIBIDO", null)
        };

        _trasladoServiceMock
            .Setup(s => s.ListarTrasladosAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(traslados);

        await _viewModel.CargarTrasladosAsync();

        _viewModel.TotalTraslados.Should().Be(2);
        _viewModel.HayTraslados.Should().BeTrue();
    }

    [Fact]
    public async Task TrasladoHistorialViewModel_CargarTraslados_WhenServiceThrows_ShouldSetError()
    {
        _trasladoServiceMock
            .Setup(s => s.ListarTrasladosAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de conexión"));

        await _viewModel.CargarTrasladosAsync();

        _viewModel.MensajeError.Should().NotBeNull();
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public void TrasladoHistorialViewModel_LimpiarFiltros_ShouldResetFilters()
    {
        _viewModel.Busqueda = "test";
        _viewModel.FiltroEstado = "SOLICITADO";
        _viewModel.FiltroOrigen = 1;
        _viewModel.FiltroDestino = 2;
        _viewModel.FechaDesde = DateTimeOffset.Now.AddDays(-30);
        _viewModel.FechaHasta = DateTimeOffset.Now;

        _viewModel.LimpiarFiltros();

        _viewModel.Busqueda.Should().Be(string.Empty);
        _viewModel.FiltroEstado.Should().Be(string.Empty);
        _viewModel.FiltroOrigen.Should().BeNull();
        _viewModel.FiltroDestino.Should().BeNull();
        _viewModel.FechaDesde.Should().BeNull();
        _viewModel.FechaHasta.Should().BeNull();
    }
}

public class AlertasCaducidadViewModelTests
{
    private readonly Mock<IAlertaAppService> _alertaServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly AlertasCaducidadViewModel _viewModel;

    public AlertasCaducidadViewModelTests()
    {
        _alertaServiceMock = new Mock<IAlertaAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _viewModel = new AlertasCaducidadViewModel(_alertaServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void AlertasCaducidadViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.AlertasCaducadas.Should().NotBeNull();
        _viewModel.AlertasCaducadas.Should().BeEmpty();
        _viewModel.AlertasProximas.Should().NotBeNull();
        _viewModel.AlertasProximas.Should().BeEmpty();
        _viewModel.TotalCaducadas.Should().Be(0);
        _viewModel.TotalProximas.Should().Be(0);
        _viewModel.TotalAlertas.Should().Be(0);
        _viewModel.NoHayAlertas.Should().BeTrue();
        _viewModel.HayAlertas.Should().BeFalse();
    }

    [Fact]
    public async Task AlertasCaducidadViewModel_CargarAlertas_ShouldPopulateLists()
    {
        var alertas = new List<AlertaDto>
        {
            new AlertaDto(1, 1, "Aspirina 500mg", 1, "Bodega Central", 1, "LOTE001", "CADUCIDAD", DateTimeOffset.Now, "Lote caducado", false),
            new AlertaDto(2, 2, "Paracetamol 500mg", 1, "Bodega Central", 2, "LOTE002", "PROXIMA_CADUCIDAD", DateTimeOffset.Now.AddDays(-1), "Lote próximo a caducar", false)
        };

        _alertaServiceMock
            .Setup(s => s.ListarAlertasAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alertas);

        await _viewModel.CargarAlertasAsync();

        _viewModel.TotalCaducadas.Should().Be(1);
        _viewModel.TotalProximas.Should().Be(1);
        _viewModel.TotalAlertas.Should().Be(2);
        _viewModel.HayAlertas.Should().BeTrue();
        _viewModel.NoHayAlertas.Should().BeFalse();
    }

    [Fact]
    public async Task AlertasCaducidadViewModel_CargarAlertas_WhenServiceThrows_ShouldSetError()
    {
        _alertaServiceMock
            .Setup(s => s.ListarAlertasAsync(1, true, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de conexión"));

        await _viewModel.CargarAlertasAsync();

        _viewModel.MensajeError.Should().NotBeNull();
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public async Task AlertasCaducidadViewModel_MarcarAtendida_ShouldRemoveFromList()
    {
        var alertaCaducada = new AlertaDto(1, 1, "Aspirina 500mg", 1, "Bodega Central", 1, "LOTE001", "CADUCIDAD", DateTimeOffset.Now, "Lote caducado", false);
        var alertaProxima = new AlertaDto(2, 2, "Paracetamol 500mg", 1, "Bodega Central", 2, "LOTE002", "PROXIMA_CADUCIDAD", DateTimeOffset.Now.AddDays(-1), "Lote próximo", false);

        _alertaServiceMock
            .Setup(s => s.ListarAlertasAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlertaDto> { alertaCaducada, alertaProxima });

        await _viewModel.CargarAlertasAsync();
        _viewModel.TotalAlertas.Should().Be(2);

        _alertaServiceMock
            .Setup(s => s.MarcarAlertaAtendidaAsync(1, 1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _viewModel.MarcarAtendidaAsync(1);

        _viewModel.TotalCaducadas.Should().Be(0);
        _viewModel.TotalProximas.Should().Be(1);
        _viewModel.TotalAlertas.Should().Be(1);
        _viewModel.MensajeExito.Should().Contain("atendida");
    }

    [Fact]
    public void AlertasCaducidadViewModel_LimpiarFiltros_ShouldResetFilters()
    {
        _viewModel.Busqueda = "test";
        _viewModel.FechaDesde = DateTimeOffset.Now.AddDays(-30);
        _viewModel.FechaHasta = DateTimeOffset.Now;
        _viewModel.FiltroUbicacion = 2;
        _viewModel.SoloNoAtendidas = false;

        _viewModel.LimpiarFiltros();

        _viewModel.Busqueda.Should().Be(string.Empty);
        _viewModel.FechaDesde.Should().BeNull();
        _viewModel.FechaHasta.Should().BeNull();
        _viewModel.FiltroUbicacion.Should().Be(1);
        _viewModel.SoloNoAtendidas.Should().BeTrue();
    }
}

public class AlertasStockViewModelTests
{
    private readonly Mock<IAlertaAppService> _alertaServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly AlertasStockViewModel _viewModel;

    public AlertasStockViewModelTests()
    {
        _alertaServiceMock = new Mock<IAlertaAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _viewModel = new AlertasStockViewModel(_alertaServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void AlertasStockViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.AlertasStock.Should().NotBeNull();
        _viewModel.AlertasStock.Should().BeEmpty();
        _viewModel.TotalAlertasStock.Should().Be(0);
        _viewModel.NoHayAlertas.Should().BeTrue();
        _viewModel.HayAlertas.Should().BeFalse();
    }

    [Fact]
    public async Task AlertasStockViewModel_CargarAlertas_ShouldPopulateList()
    {
        var alertas = new List<AlertaDto>
        {
            new AlertaDto(1, 1, "Aspirina 500mg", 1, "Bodega Central", null, null, "STOCK_MINIMO", DateTimeOffset.Now, "Stock por debajo del mínimo", false)
        };

        _alertaServiceMock
            .Setup(s => s.ListarAlertasAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alertas);

        await _viewModel.CargarAlertasAsync();

        _viewModel.TotalAlertasStock.Should().Be(1);
        _viewModel.HayAlertas.Should().BeTrue();
        _viewModel.NoHayAlertas.Should().BeFalse();
    }

    [Fact]
    public async Task AlertasStockViewModel_MarcarAtendida_ShouldRemoveFromList()
    {
        var alerta = new AlertaDto(1, 1, "Aspirina 500mg", 1, "Bodega Central", null, null, "STOCK_MINIMO", DateTimeOffset.Now, "Stock mínimo", false);

        _alertaServiceMock
            .Setup(s => s.ListarAlertasAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlertaDto> { alerta });

        await _viewModel.CargarAlertasAsync();
        _viewModel.TotalAlertasStock.Should().Be(1);

        _alertaServiceMock
            .Setup(s => s.MarcarAlertaAtendidaAsync(1, 1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _viewModel.MarcarAtendidaAsync(1);

        _viewModel.TotalAlertasStock.Should().Be(0);
        _viewModel.NoHayAlertas.Should().BeTrue();
    }

    [Fact]
    public void AlertasStockViewModel_LimpiarFiltros_ShouldResetFilters()
    {
        _viewModel.Busqueda = "test";
        _viewModel.FechaDesde = DateTimeOffset.Now.AddDays(-30);
        _viewModel.FechaHasta = DateTimeOffset.Now;
        _viewModel.FiltroUbicacion = 2;
        _viewModel.SoloNoAtendidas = false;

        _viewModel.LimpiarFiltros();

        _viewModel.Busqueda.Should().Be(string.Empty);
        _viewModel.FechaDesde.Should().BeNull();
        _viewModel.FechaHasta.Should().BeNull();
        _viewModel.FiltroUbicacion.Should().Be(1);
        _viewModel.SoloNoAtendidas.Should().BeTrue();
    }
}

public class FEFOViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly FEFOViewModel _viewModel;

    public FEFOViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _viewModel = new FEFOViewModel(_inventarioServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public void FEFOViewModel_InitialState_ShouldBeEmpty()
    {
        _viewModel.LotesFEFO.Should().NotBeNull();
        _viewModel.LotesFEFO.Should().BeEmpty();
        _viewModel.TotalLotes.Should().Be(0);
        _viewModel.LotesElegibles.Should().Be(0);
        _viewModel.LotesCaducados.Should().Be(0);
        _viewModel.NoHayLotes.Should().BeTrue();
    }

    [Fact]
    public async Task FEFOViewModel_CargarLotes_ShouldSortByFEFO()
    {
        var existencias = new List<ExistenciaDto>
        {
            new ExistenciaDto(1, "Producto A", "ABC", 1, "LOTE001", DateOnly.FromDateTime(DateTime.Now.AddDays(60)), 1, "Bodega Central", 100, 0, 100),
            new ExistenciaDto(2, "Producto B", "DEF", 2, "LOTE002", DateOnly.FromDateTime(DateTime.Now.AddDays(5)), 1, "Bodega Central", 50, 0, 50),
            new ExistenciaDto(3, "Producto C", "GHI", 3, "LOTE003", DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), 1, "Bodega Central", 0, 0, 0)
        };

        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(null, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existencias);

        await _viewModel.CargarLotesFEFOAsync();

        _viewModel.TotalLotes.Should().Be(3);
        _viewModel.LotesCaducados.Should().Be(1);
        _viewModel.LotesElegibles.Should().Be(1); // Solo LOTE002 (5 días, disponible > 0)

        // El primer lote debería ser el más cercano a caducar (LOTE002 con 5 días)
        var primerLote = _viewModel.LotesFEFO.First();
        primerLote.NumeroLote.Should().Be("LOTE002");
    }

    [Fact]
    public void FEFOViewModel_LimpiarFiltros_ShouldResetFilters()
    {
        _viewModel.Busqueda = "test";
        _viewModel.FiltroEstado = "VIGENTE";
        _viewModel.FiltroUbicacion = 2;
        _viewModel.FechaCaducidadDesde = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        _viewModel.FechaCaducidadHasta = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        _viewModel.SoloElegibles = true;

        _viewModel.LimpiarFiltros();

        _viewModel.Busqueda.Should().Be(string.Empty);
        _viewModel.FiltroEstado.Should().Be(string.Empty);
        _viewModel.FiltroUbicacion.Should().Be(1);
        _viewModel.FechaCaducidadDesde.Should().BeNull();
        _viewModel.FechaCaducidadHasta.Should().BeNull();
        _viewModel.SoloElegibles.Should().BeFalse();
    }
}

public class DashboardViewModelTests
{
    private readonly Mock<IInventarioAppService> _inventarioServiceMock;
    private readonly Mock<IAlertaAppService> _alertaServiceMock;
    private readonly Mock<ITrasladoAppService> _trasladoServiceMock;
    private readonly Mock<ISesionAppService> _sesionServiceMock;
    private readonly DashboardViewModel _viewModel;

    public DashboardViewModelTests()
    {
        _inventarioServiceMock = new Mock<IInventarioAppService>();
        _alertaServiceMock = new Mock<IAlertaAppService>();
        _trasladoServiceMock = new Mock<ITrasladoAppService>();
        _sesionServiceMock = new Mock<ISesionAppService>();
        _sesionServiceMock.Setup(s => s.IdUbicacion).Returns(1);
        _sesionServiceMock.Setup(s => s.IdUsuario).Returns(1);

        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(null, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExistenciaDto>
            {
                new ExistenciaDto(1, "Aspirina 500mg", "ABC", 1, "LOTE001", DateOnly.FromDateTime(DateTime.Now.AddDays(30)), 1, "Bodega Central", 100, 0, 100)
            });

        _inventarioServiceMock
            .Setup(s => s.ListarLotesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LoteDto>
            {
                new LoteDto(1, 1, "Aspirina 500mg", "ABC", "LOTE001", DateOnly.FromDateTime(DateTime.Now.AddDays(10)), DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), "VIGENTE")
            });

        _alertaServiceMock
            .Setup(s => s.ListarAlertasAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlertaDto>());

        _trasladoServiceMock
            .Setup(s => s.ListarTrasladosAsync("SOLICITADO", 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrasladoDto>());

        _inventarioServiceMock
            .Setup(s => s.ConsultarMovimientosAsync(null, 1, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MovimientoDto>());

        _inventarioServiceMock
            .Setup(s => s.ListarRecepcionesAsync(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MovimientoDto>());

        _viewModel = new DashboardViewModel(_inventarioServiceMock.Object, _alertaServiceMock.Object, _trasladoServiceMock.Object, _sesionServiceMock.Object);
    }

    [Fact]
    public async Task DashboardViewModel_CargarDatos_ShouldPopulateIndicators()
    {
        await _viewModel.CargarDatosInicialesAsync();

        _viewModel.TotalProductos.Should().Be(1);
        _viewModel.TotalUnidadesBodega.Should().Be(100);
        _viewModel.AlertasPendientes.Should().Be(0);
        _viewModel.EstaCargando.Should().BeFalse();
    }

    [Fact]
    public async Task DashboardViewModel_CargarDatos_WhenServiceThrows_ShouldSetError()
    {
        _inventarioServiceMock
            .Setup(s => s.ConsultarExistenciasAsync(null, 1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de conexión"));

        await _viewModel.CargarDatosInicialesAsync();

        _viewModel.MensajeError.Should().NotBeNull();
        _viewModel.EstaCargando.Should().BeFalse();
    }
}


