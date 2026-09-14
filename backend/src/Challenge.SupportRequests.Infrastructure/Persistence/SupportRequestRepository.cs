using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Domain.Entities;
using Dapper;

namespace Challenge.SupportRequests.Infrastructure.Persistence;

public sealed class SupportRequestRepository(SqlConnectionFactory connectionFactory) : ISupportRequestRepository
{
    private const string Projection = """
        IdSolicitacao AS Id, Titulo AS Title, Descricao AS Description, Solicitante AS Requester,
        Prioridade AS Priority, Status AS Status, DataCriacao AS CreatedAt, DataConclusao AS CompletedAt
        """;

    public async Task<long> CreateAsync(SupportRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Solicitacoes (Titulo, Descricao, Solicitante, Prioridade, Status, DataCriacao, DataConclusao)
            OUTPUT INSERTED.IdSolicitacao
            VALUES (@Title, @Description, @Requester, @Priority, @Status, @CreatedAt, @CompletedAt);
            """;
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, request, cancellationToken: cancellationToken));
    }

    public async Task<SupportRequest?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<SupportRequestPersistenceModel>(
            new CommandDefinition($"SELECT {Projection} FROM dbo.Solicitacoes WHERE IdSolicitacao = @Id;",
                new { Id = id }, cancellationToken: cancellationToken));
        return row?.ToDomain();
    }

    public async Task<SupportRequestPage> ListAsync(SupportRequestFilter filter, CancellationToken cancellationToken)
    {
        const string predicate = """
            WHERE (@Status IS NULL OR Status = @Status)
              AND (@Priority IS NULL OR Prioridade = @Priority)
              AND (@SearchPattern IS NULL
                OR Titulo COLLATE Latin1_General_100_CI_AS LIKE @SearchPattern ESCAPE '~'
                OR Descricao COLLATE Latin1_General_100_CI_AS LIKE @SearchPattern ESCAPE '~'
                OR Solicitante COLLATE Latin1_General_100_CI_AS LIKE @SearchPattern ESCAPE '~')
            """;
        var sql = $"""
            SELECT {Projection} FROM dbo.Solicitacoes {predicate}
            ORDER BY DataCriacao DESC, IdSolicitacao DESC
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            SELECT COUNT_BIG(*) FROM dbo.Solicitacoes {predicate};
            """;
        var parameters = new
        {
            filter.Status,
            filter.Priority,
            filter.Limit,
            Offset = ((long)filter.Page - 1) * filter.Limit,
            SearchPattern = filter.Search is null ? null : "%" + EscapeLike(filter.Search) + "%"
        };
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        using var results = await connection.QueryMultipleAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        var items = (await results.ReadAsync<SupportRequestPersistenceModel>()).Select(x => x.ToDomain()).ToArray();
        return new(items, await results.ReadSingleAsync<long>());
    }

    public Task<bool> UpdatePriorityAsync(SupportRequest request, CancellationToken cancellationToken)
        => ExecuteWriteAsync("UPDATE dbo.Solicitacoes SET Prioridade = @Priority WHERE IdSolicitacao = @Id;", request, cancellationToken);

    public Task<bool> UpdateStatusAsync(SupportRequest request, CancellationToken cancellationToken)
        => ExecuteWriteAsync("UPDATE dbo.Solicitacoes SET Status = @Status, DataConclusao = @CompletedAt WHERE IdSolicitacao = @Id;", request, cancellationToken);

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
        => ExecuteWriteAsync("DELETE FROM dbo.Solicitacoes WHERE IdSolicitacao = @Id;", new { Id = id }, cancellationToken);

    private async Task<bool> ExecuteWriteAsync(string sql, object parameters, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        return await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)) == 1;
    }

    private static string EscapeLike(string value) => value.Replace("~", "~~").Replace("%", "~%").Replace("_", "~_").Replace("[", "~[");
}
