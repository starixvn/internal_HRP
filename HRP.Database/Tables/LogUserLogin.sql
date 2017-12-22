SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[LogUserLogin](
	[UserId] [int] NOT NULL,
	[LogTime] [datetime] NOT NULL,
	[IP] [varchar](30) NULL,
	[Platform] [varchar](255) NULL,
	[ActionTypeId] [tinyint] NULL
) ON [HRP_Data]

GO

SET ANSI_PADDING OFF
GO


