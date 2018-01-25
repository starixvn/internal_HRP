CREATE TABLE [dbo].[LogDetail] (
    [LogId]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [ActionId]     SMALLINT       NOT NULL,
    [UserId]       INT            NOT NULL,
    [ObjectId]     INT            NULL,
    [ObjectTypeId] INT            NULL,
    [LogDate]      DATETIME       NOT NULL,
    [Duration]     INT            NULL,
    [Data]         NVARCHAR (MAX) NULL,
    [OldData]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LogDetail] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

