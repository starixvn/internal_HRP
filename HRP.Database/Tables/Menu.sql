SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Menu](
	[MenuId] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Description] [nvarchar](200) NULL,
	[URL] [varchar](200) NULL,
	[ParentId] [smallint] NULL,
	[FunctionId] [smallint] NULL,
	[DisplayOrder] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [HRP_Data]
) ON [HRP_Data]

GO

SET ANSI_PADDING OFF
GO


