using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Infrastructure.Clients;
using Altamira.Bodega.Infrastructure.Services;
using Altamira.Bodega.Presentation.Services;
using Altamira.Bodega.Presentation.ViewModels;
using Altamira.Bodega.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Altamira.Bodega.Presentation.DependencyInjection;

public static class PresentationDependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<INavigationService>(sp => new NavigationService(viewType =>
        {
            return viewType switch
            {
                var _ when viewType == typeof(ShellView) => new ShellView(sp),
                var _ when viewType == typeof(DashboardView) => new DashboardView(),
                var _ when viewType == typeof(ExistenciasView) => new ExistenciasView(),
                var _ when viewType == typeof(CatalogoView) => new CatalogoView(),
                var _ when viewType == typeof(LotesView) => new LotesView(),
                var _ when viewType == typeof(LoteInfoView) => new LoteInfoView(),
                var _ when viewType == typeof(KardexView) => new KardexView(),
                var _ when viewType == typeof(AjustesView) => new AjustesView(),
                var _ when viewType == typeof(RecepcionView) => new RecepcionView(),
                var _ when viewType == typeof(RecepcionHistorialView) => new RecepcionHistorialView(),
                var _ when viewType == typeof(EntradasView) => new EntradasView(),
                var _ when viewType == typeof(SalidasView) => new SalidasView(),
                var _ when viewType == typeof(TrasladosView) => new TrasladoCrearView(),
                var _ when viewType == typeof(TrasladoDespachoView) => new TrasladoDespachoView(),
                var _ when viewType == typeof(TrasladoRecepcionView) => new TrasladoRecepcionView(),
                var _ when viewType == typeof(TrasladoHistorialView) => new TrasladoHistorialView(),
                var _ when viewType == typeof(AlertasCaducidadView) => new AlertasCaducidadView(),
                var _ when viewType == typeof(AlertasStockView) => new AlertasStockView(),
                var _ when viewType == typeof(FEFOView) => new FEFOView(),
                var _ when viewType == typeof(FEFOView) => new FEFOView(),
                var _ when viewType == typeof(BitacoraView) => new BitacoraView(),
                var _ when viewType == typeof(UsuariosView) => new UsuariosView(),
                _ => throw new InvalidOperationException($"Vista no registrada: {viewType.Name}")
            };
        }));

        services.AddTransient<ShellViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ExistenciasViewModel>();
        services.AddTransient<ProductoViewModel>();
        services.AddTransient<LoteInfoViewModel>();
        services.AddTransient<KardexViewModel>();
        services.AddTransient<AjustesViewModel>();
        services.AddTransient<RecepcionViewModel>();
        services.AddTransient<EntradasViewModel>();
        services.AddTransient<SalidasViewModel>();
        services.AddTransient<TrasladoCrearViewModel>();
        services.AddTransient<TrasladoDespachoViewModel>();
        services.AddTransient<TrasladoRecepcionViewModel>();
        services.AddTransient<TrasladoHistorialViewModel>();
        services.AddTransient<AlertasCaducidadViewModel>();
        services.AddTransient<AlertasStockViewModel>();
        services.AddTransient<FEFOViewModel>();
        services.AddTransient<BitacoraViewModel>();
        services.AddTransient<UsuariosViewModel>();
        services.AddTransient<RecepcionHistorialViewModel>();

        return services;
    }
}
