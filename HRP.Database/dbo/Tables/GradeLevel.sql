CREATE TABLE [dbo].[GradeLevel] (
    [GradeLevelId]  SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50)  NOT NULL,
    [JobCategoryId] SMALLINT       NOT NULL,
    [Description]   NVARCHAR (200) NULL,
    [IsActive]      BIT            NOT NULL,
    CONSTRAINT [PK_GradeLevel] PRIMARY KEY CLUSTERED ([GradeLevelId] ASC)
);

