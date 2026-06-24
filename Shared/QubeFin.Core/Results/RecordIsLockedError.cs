using FluentResults;

namespace QubeFin.Core.Results;

public class RecordIsLockedError(string message) : Error(message) { }