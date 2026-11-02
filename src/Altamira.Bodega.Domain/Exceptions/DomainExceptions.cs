namespace Altamira.Bodega.Domain.Exceptions;

public sealed class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string message) : base(message) { }
}

public sealed class NoEncontradoException : Exception
{
    public NoEncontradoException(string message) : base(message) { }
}

public sealed class DatosInvalidosException : Exception
{
    public DatosInvalidosException(string message) : base(message) { }
}

public sealed class SesionExpiradaException : Exception
{
    public SesionExpiradaException(string message = "La sesión ha expirado. Inicie sesión de nuevo.") : base(message) { }
}

public sealed class SinPermisosException : Exception
{
    public SinPermisosException(string message = "No tiene permisos para realizar esta operación.") : base(message) { }
}

public sealed class ErrorInternoException : Exception
{
    public ErrorInternoException(string message = "Error interno del servidor.") : base(message) { }
}

public sealed class ConflictoConcurrenciaException : Exception
{
    public ConflictoConcurrenciaException(string message = "El registro fue modificado por otro usuario.") : base(message) { }
}
