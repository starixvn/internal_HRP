CREATE TABLE [dbo].[Section] (
    [SectionId]    SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [CostCenterId] SMALLINT       NULL,
    [IsActive]     BIT            NOT NULL,
    CONSTRAINT [PK_Section] PRIMARY KEY CLUSTERED ([SectionId] ASC)
);



