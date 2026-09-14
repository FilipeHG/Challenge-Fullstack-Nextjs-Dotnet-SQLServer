namespace Challenge.SupportRequests.Domain.Exceptions;

public sealed class SupportRequestDeletionNotAllowedException() : Exception("Apenas solicitações abertas podem ser excluídas.");
