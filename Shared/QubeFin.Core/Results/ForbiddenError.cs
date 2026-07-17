using FluentResults;

namespace QubeFin.Core.Results;

public class ForbiddenError(string message) : Error(message) { }
