CREATE TABLE [dbo].[LogUserLogin] (
    [UserId]       INT           NOT NULL,
    [LogTime]      DATETIME      NOT NULL,
    [IP]           VARCHAR (30)  NULL,
    [Platform]     VARCHAR (255) NULL,
    [ActionTypeId] TINYINT       NULL
);

