using FluentResults;

namespace QubeFin.Core.Results;

public class ValidationError(string message) : Error(message) { }