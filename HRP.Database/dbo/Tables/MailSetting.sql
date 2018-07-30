CREATE TABLE [dbo].[MailSetting] (
    [CompanyId] SMALLINT       NOT NULL,
    [Server]    NVARCHAR (250) NULL,
    [Port]      INT            NULL,
    [Username]  NVARCHAR (100) NULL,
    [Password]  NVARCHAR (100) NULL,
    [IsAuth]    BIT            NULL,
    [Priority]  TINYINT        NULL,
    CONSTRAINT [PK_MailSetting] PRIMARY KEY CLUSTERED ([CompanyId] ASC)
);

