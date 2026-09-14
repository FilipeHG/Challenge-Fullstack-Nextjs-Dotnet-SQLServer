using System.Text.RegularExpressions;
using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;
using Challenge.SupportRequests.Infrastructure.Persistence;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Challenge.SupportRequests.IntegrationTests.Persistence;

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SUPPORT_REQUESTS_TEST_SQL")))
            Skip = "Opt-in: set SUPPORT_REQUESTS_TEST_SQL to a local SQL Server connection with database creation permissions.";
    }
}

public sealed class SqlRepositoryTests
{
    [SqlServerFact]
    [Trait("Category", "SqlServer")]
    public async Task Repository_ShouldPersistFilterPageUpdateAndDelete_InIsolatedDatabase()
    {
        var name = "SupportRequestsTest_" + Guid.NewGuid().ToString("N");
        var connectionString = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("SUPPORT_REQUESTS_TEST_SQL")) { InitialCatalog = "master" };
        await using var admin = new SqlConnection(connectionString.ConnectionString);
        await admin.OpenAsync();
        var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "001_create_database_and_solicitacoes.sql"));
        script = script.Replace("SupportRequestsDb", name);
        try
        {
            // Run twice to verify the versioned migration is safe to rerun.
            for (var pass = 0; pass < 2; pass++)
                foreach (var batch in Regex.Split(script, @"^GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                    if (!string.IsNullOrWhiteSpace(batch)) await admin.ExecuteAsync(batch);

            connectionString.InitialCatalog = name;
            var repository = new SupportRequestRepository(new SqlConnectionFactory(connectionString.ConnectionString));
            var now = new DateTime(2026, 9, 14, 12, 0, 0, DateTimeKind.Utc);
            var first = SupportRequest.Create("VPN 100% outage", "Network access fails", "Maria", Priority.High, now);
            first.AssignId(await repository.CreateAsync(first, default));
            var second = SupportRequest.Create("Printer offline", "VPN document printing", "John", Priority.Low, now);
            second.AssignId(await repository.CreateAsync(second, default));

            var loaded = await repository.GetByIdAsync(first.Id, default);
            Assert.NotNull(loaded);
            Assert.Equal(first.Title, loaded.Title);
            Assert.Equal(DateTimeKind.Utc, loaded.CreatedAt.Kind);
            Assert.Equal(Priority.High, loaded.Priority);
            Assert.Null(loaded.CompletedAt);

            var page = await repository.ListAsync(new(null, null, null, 1, 1), default);
            Assert.Equal(2, page.TotalItems);
            Assert.Equal(second.Id, Assert.Single(page.Items).Id);
            var nextPage = await repository.ListAsync(new(null, null, null, 2, 1), default);
            Assert.Equal(first.Id, Assert.Single(nextPage.Items).Id);
            Assert.Single((await repository.ListAsync(new(RequestStatus.Open, Priority.High, "vpn", 1, 100), default)).Items);
            Assert.Single((await repository.ListAsync(new(null, null, "MARIA", 1, 100), default)).Items);
            Assert.Single((await repository.ListAsync(new(null, null, "100%", 1, 100), default)).Items);
            Assert.Empty((await repository.ListAsync(new(null, null, "'; DROP TABLE Solicitacoes;--", 1, 100), default)).Items);
            Assert.Empty((await repository.ListAsync(new(null, null, null, int.MaxValue, 100), default)).Items);

            first.ChangeStatus(RequestStatus.Completed, now.AddMinutes(1));
            Assert.True(await repository.UpdateStatusAsync(first, default));
            loaded = await repository.GetByIdAsync(first.Id, default);
            Assert.Equal(now.AddMinutes(1), loaded!.CompletedAt);
            Assert.Equal(DateTimeKind.Utc, loaded.CompletedAt!.Value.Kind);
            first.ChangeStatus(RequestStatus.InProgress, now.AddMinutes(2));
            Assert.True(await repository.UpdateStatusAsync(first, default));
            Assert.Null((await repository.GetByIdAsync(first.Id, default))!.CompletedAt);
            first.ChangePriority(Priority.Medium);
            Assert.True(await repository.UpdatePriorityAsync(first, default));
            Assert.Equal(Priority.Medium, (await repository.GetByIdAsync(first.Id, default))!.Priority);

            second.EnsureCanBeDeleted();
            Assert.True(await repository.DeleteAsync(second.Id, default));
            Assert.Null(await repository.GetByIdAsync(second.Id, default));
            Assert.False(await repository.DeleteAsync(second.Id, default));
            Assert.Equal(1, await admin.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_Solicitacoes_Status_Prioridade_DataCriacao';"));
            await Assert.ThrowsAsync<SqlException>(() => admin.ExecuteAsync(
                "UPDATE dbo.Solicitacoes SET Prioridade = 9 WHERE IdSolicitacao = @Id;", new { first.Id }));
        }
        finally
        {
            await admin.ChangeDatabaseAsync("master");
            SqlConnection.ClearAllPools();
            // The identifier is exclusively a generated GUID, never an external value.
            await admin.ExecuteAsync($"IF DB_ID(N'{name}') IS NOT NULL BEGIN ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]; END;");
        }
    }
}
