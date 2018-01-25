CREATE TABLE [dbo].[UserFunction] (
    [UserId]     INT      NOT NULL,
    [FunctionId] SMALLINT NOT NULL,
    [CompanyId]  SMALLINT NOT NULL,
    [IsRead]     BIT      NOT NULL,
    [IsHost]     BIT      NOT NULL,
    CONSTRAINT [PK_UserFunction] PRIMARY KEY CLUSTERED ([UserId] ASC, [FunctionId] ASC, [CompanyId] ASC)
);

