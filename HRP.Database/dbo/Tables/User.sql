CREATE TABLE [dbo].[User] (
    [UserId]             INT           IDENTITY (1, 1) NOT NULL,
    [Username]           VARCHAR (30)  NOT NULL,
    [FullName]           NVARCHAR (70) NULL,
    [Phone]              VARCHAR (20)  NULL,
    [Email]              VARCHAR (254) NOT NULL,
    [Password]           VARCHAR (200) NOT NULL,
    [IsActive]           BIT           NOT NULL,
    [LastLoginDate]      DATETIME      NULL,
    [IsOTPRegistered]    BIT           NOT NULL,
    [SecretKey]          VARCHAR (250) NULL,
    [CreatedBy]          INT           NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [TokenResetPassword] VARCHAR (250) NULL,
    [TokenOTPRegister]   VARCHAR (250) NULL,
    [IsSystemAdmin]      BIT           DEFAULT ((0)) NOT NULL,
	FilePassword		 VARCHAR(MAX)
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

