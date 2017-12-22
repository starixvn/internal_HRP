SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LogDetail](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[ActionId] [smallint] NOT NULL,
	[UserId] [int] NOT NULL,
	[ObjectId] [int] NULL,
	[ObjectTypeId] [int] NULL,
	[LogDate] [datetime] NOT NULL,
	[Duration] [int] NULL,
	[Data] [nvarchar](max) NULL,
	[OldData] [nvarchar](max) NULL,
 CONSTRAINT [PK_LogDetail] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [HRP_Data]
) ON [HRP_Data] TEXTIMAGE_ON [HRP_Data]

GO


