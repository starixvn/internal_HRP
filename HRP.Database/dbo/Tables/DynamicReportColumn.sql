CREATE TABLE [dbo].[DynamicReportColumn] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [ItemId]      INT           NOT NULL,
    [HeaderName]  VARCHAR (100) NOT NULL,
    [ColumnName]  VARCHAR (100) NOT NULL,
    [IsEncrypted] BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

