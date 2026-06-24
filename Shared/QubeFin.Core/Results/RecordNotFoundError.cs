using FluentResults;

namespace QubeFin.Core.Results;

public class RecordNotFoundError(string message) : Error(message) { }