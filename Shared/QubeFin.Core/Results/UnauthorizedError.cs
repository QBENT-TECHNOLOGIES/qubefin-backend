using FluentResults;

namespace QubeFin.Core.Results;

public class UnauthorizedError(string message) : Error(message) { }
