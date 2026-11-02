using Altamira.Bodega.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Altamira.Bodega.Infrastructure.Services;

/// <summary>
/// Mapeador de errores de la API central a excepciones de dominio del cliente WPF.
/// Centraliza la interpretación de códigos de estado HTTP y códigos de error de la API.
/// </summary>
public static class ApiExceptionMapper
{
    public static Exception Map(HttpResponseMessage response, string endpoint, ILogger logger)
    {
        var statusCode = (int)response.StatusCode;
        logger.LogWarning("API error {StatusCode} at {Endpoint}", statusCode, endpoint);

        return statusCode switch
        {
            400 => new DatosInvalidosException($"Datos inválidos en {endpoint}."),
            401 => new SesionExpiradaException(),
            403 => new SinPermisosException(),
            404 => new NoEncontradoException($"Recurso no encontrado en {endpoint}."),
            409 => new ConflictoConcurrenciaException($"Conflicto de concurrencia en {endpoint}."),
            422 => new ReglaNegocioException($"Regla de negocio violada en {endpoint}."),
            500 or >= 500 => new ErrorInternoException($"Error del servidor en {endpoint}."),
            _ => new ErrorInternoException($"Error inesperado ({statusCode}) en {endpoint}.")
        };
    }
}
