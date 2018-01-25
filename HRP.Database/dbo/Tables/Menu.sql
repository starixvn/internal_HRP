CREATE TABLE [dbo].[Menu] (
    [MenuId]       SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)   NULL,
    [Description]  NVARCHAR (200) NULL,
    [Link]         VARCHAR (200)  NULL,
    [ParentId]     SMALLINT       NULL,
    [FunctionId]   SMALLINT       NULL,
    [DisplayOrder] SMALLINT       NOT NULL,
    [IsActive]     BIT            NOT NULL,
    CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED ([MenuId] ASC)
);

