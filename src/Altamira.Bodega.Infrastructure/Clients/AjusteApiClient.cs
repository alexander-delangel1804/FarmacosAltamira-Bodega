using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class AjusteApiClient : BaseApiClient, IAjusteService
{
    private readonly string _baseUrl;

    public AjusteApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<AjusteApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<AjusteDto> RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/ajustes";
        var result = await PostAsync<AjusteDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error registrando ajuste");
        return result.Dato!;
    }
}
