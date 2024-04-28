CREATE TABLE [dbo].[TempFile] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Timestamp] DATETIME NOT NULL,
    [PrincipalId] UNIQUEIDENTIFIER NULL,
    [FileName] NVARCHAR(255) NOT NULL,
    [ContentType] VARCHAR(255) NOT NULL,
    [Data] VARBINARY(MAX) NULL,
    [Label] VARCHAR(512) NULL,

    CONSTRAINT [PK_TempFile] PRIMARY KEY NONCLUSTERED ([Id])
)
GO

DROP TABLE [dbo].[Report]
DROP TABLE [dbo].[CalculationResult]
