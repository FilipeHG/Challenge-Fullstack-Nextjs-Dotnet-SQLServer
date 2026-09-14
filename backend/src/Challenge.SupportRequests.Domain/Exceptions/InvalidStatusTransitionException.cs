namespace Challenge.SupportRequests.Domain.Exceptions;

public sealed class InvalidStatusTransitionException() : Exception("Uma solicitação concluída não pode voltar para aberta.");
