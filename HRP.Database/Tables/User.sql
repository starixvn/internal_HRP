SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](30) NOT NULL,
	[FullName] [nvarchar](70) NULL,
	[Phone] [varchar](20) NULL,
	[Email] [varchar](254) NOT NULL,
	[Password] [varchar](200) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[LastLoginDate] [datetime] NULL,
	[IsOTPRegistered] [bit] NOT NULL,
	[SecretKey] [varchar](250) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[TokenResetPassword] [varchar](250) NULL,
	[TokenOTPRegister] [varchar](250) NULL,
	[IsSystemAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [HRP_Data]
) ON [HRP_Data]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsSystemAdmin]
GO


