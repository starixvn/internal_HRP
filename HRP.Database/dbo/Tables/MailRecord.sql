CREATE TABLE [dbo].[MailRecord] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [MailTo]       VARCHAR (254)   NULL,
    [Subject]      NVARCHAR (200)  NULL,
    [SentTime]     DATETIME        NULL,
    [IsSuccessful] BIT             NULL,
    [ErrorMessage] NVARCHAR (1000) NULL,
    [StackTrace]   NVARCHAR (2000) NULL,
    CONSTRAINT [PK_MailRecord] PRIMARY KEY CLUSTERED ([Id] ASC)
);

