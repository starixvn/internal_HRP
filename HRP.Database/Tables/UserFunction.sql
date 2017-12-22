SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserFunction](
	[UserId] [int] NOT NULL,
	[FunctionId] [smallint] NOT NULL,
	[CompanyId] [smallint] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[IsHost] [bit] NOT NULL,
 CONSTRAINT [PK_UserFunction] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[FunctionId] ASC,
	[CompanyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [HRP_Data]
) ON [HRP_Data]

GO


