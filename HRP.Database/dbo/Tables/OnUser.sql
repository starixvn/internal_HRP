CREATE TABLE [dbo].[OnUser] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [UserId]       INT           NOT NULL,
    [ReloadTime]   DATETIME      NULL,
    [IP]           VARCHAR (50)  NULL,
    [Platform]     VARCHAR (255) NULL,
    [NetSessionId] VARCHAR (50)  NULL,
    [IsKickedOut]  BIT           NOT NULL,
    CONSTRAINT [PK_OnUser] PRIMARY KEY CLUSTERED ([Id] ASC)
);

