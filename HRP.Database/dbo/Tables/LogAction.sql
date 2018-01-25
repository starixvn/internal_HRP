CREATE TABLE [dbo].[LogAction] (
    [ActionId]     SMALLINT       IDENTITY (1, 1) NOT NULL,
    [ActionName]   NVARCHAR (200) NOT NULL,
    [ActionTypeId] TINYINT        NOT NULL,
    [SPName]       VARCHAR (50)   NOT NULL,
    [Items]        VARCHAR (MAX)  NOT NULL,
    [SubItems]     VARCHAR (MAX)  NULL,
    CONSTRAINT [PK_LogAction] PRIMARY KEY CLUSTERED ([ActionId] ASC)
);

