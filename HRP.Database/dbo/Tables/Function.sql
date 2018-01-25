CREATE TABLE [dbo].[Function] (
    [FunctionId]  SMALLINT       IDENTITY (1, 1) NOT NULL,
    [FunctionKey] VARCHAR (50)   NOT NULL,
    [Name]        NVARCHAR (200) NOT NULL,
    [ParentId]    SMALLINT       NULL,
    [IsReadOnly]  BIT            NOT NULL,
    [IsActive]    BIT            NOT NULL,
    [IsAdmin]     BIT            NOT NULL,
    CONSTRAINT [PK_Function] PRIMARY KEY CLUSTERED ([FunctionId] ASC)
);

