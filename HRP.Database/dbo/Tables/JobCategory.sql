CREATE TABLE [dbo].[JobCategory] (
    [JobCategoryId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50)  NOT NULL,
    [Description]   NVARCHAR (200) NULL,
    [IsActive]      BIT            NOT NULL,
    CONSTRAINT [PK_JobCategory] PRIMARY KEY CLUSTERED ([JobCategoryId] ASC)
);

