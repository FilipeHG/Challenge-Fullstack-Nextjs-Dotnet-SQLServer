IF DB_ID(N'SupportRequestsDb') IS NULL
    CREATE DATABASE SupportRequestsDb;
GO
USE SupportRequestsDb;
GO
IF OBJECT_ID(N'dbo.Solicitacoes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Solicitacoes
    (
        IdSolicitacao BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Solicitacoes PRIMARY KEY CLUSTERED,
        Titulo NVARCHAR(200) NOT NULL,
        Descricao NVARCHAR(4000) NOT NULL,
        Solicitante NVARCHAR(200) NOT NULL,
        Prioridade TINYINT NOT NULL CONSTRAINT CK_Solicitacoes_Prioridade CHECK (Prioridade BETWEEN 1 AND 3),
        Status TINYINT NOT NULL CONSTRAINT CK_Solicitacoes_Status CHECK (Status BETWEEN 1 AND 3),
        DataCriacao DATETIME2(3) NOT NULL CONSTRAINT DF_Solicitacoes_DataCriacao DEFAULT SYSUTCDATETIME(),
        DataConclusao DATETIME2(3) NULL
    );
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Solicitacoes')
    AND name = N'IX_Solicitacoes_Status_Prioridade_DataCriacao')
    CREATE INDEX IX_Solicitacoes_Status_Prioridade_DataCriacao
        ON dbo.Solicitacoes (Status, Prioridade, DataCriacao DESC, IdSolicitacao DESC);
GO
