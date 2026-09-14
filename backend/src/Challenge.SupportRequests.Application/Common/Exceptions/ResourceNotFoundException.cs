namespace Challenge.SupportRequests.Application.Common.Exceptions;

public sealed class ResourceNotFoundException(long id) : Exception($"Solicitação {id} não encontrada.");
