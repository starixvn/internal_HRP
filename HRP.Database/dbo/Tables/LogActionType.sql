CREATE TABLE [dbo].[LogActionType] (
    [ActionTypeId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NULL,
    CONSTRAINT [PK_LogActionType] PRIMARY KEY CLUSTERED ([ActionTypeId] ASC)
);

