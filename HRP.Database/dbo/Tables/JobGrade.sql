CREATE TABLE [dbo].[JobGrade] (
    [JobGradeId]   SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [GradeLevelId] SMALLINT       NOT NULL,
    [Description]  NVARCHAR (200) NULL,
    [IsActive]     BIT            NOT NULL,
    CONSTRAINT [PK_JobGrade] PRIMARY KEY CLUSTERED ([JobGradeId] ASC)
);

