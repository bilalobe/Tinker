namespace Tinker.Shared.Exceptions;

public record ValidationError(string Property, string Message);