CREATE TABLE [dbo].[CostCenter] (
    [CostCenterId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]         VARCHAR (10)   NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [DepartmentId] SMALLINT       NOT NULL,
    [IsActive]     BIT            NOT NULL,
    CONSTRAINT [PK_CostCenter] PRIMARY KEY CLUSTERED ([CostCenterId] ASC)
);

