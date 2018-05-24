CREATE TABLE [dbo].[MailError] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [MailTo]       VARCHAR (254)   NULL,
    [Subject]      NVARCHAR (200)  NULL,
    [ErrorMessage] NVARCHAR (1000) NULL,
    [StackTrace]   NVARCHAR (2000) NULL,
    [ErrorTime]    DATETIME        NULL,
    CONSTRAINT [PK_MailError] PRIMARY KEY CLUSTERED ([Id] ASC)
);

