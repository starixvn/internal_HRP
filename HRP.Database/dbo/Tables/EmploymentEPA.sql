CREATE TABLE [dbo].[EmploymentEPA] (
    [EmployeeId]            INT           NOT NULL,
    [FullNameAdministrator] NVARCHAR (70) NOT NULL,
    [NicknameAdministrator] VARCHAR (30)  NULL,
    [FullNameEvaluator1]    NVARCHAR (70) NULL,
    [NicknameEvaluator1]    VARCHAR (30)  NULL,
    [FullNameEvaluator2]    NVARCHAR (70) NULL,
    [NicknameEvaluator2]    VARCHAR (30)  NULL,
    [FullNameReviewer]      NVARCHAR (70) NULL,
    [NicknameReviewer]      VARCHAR (30)  NULL,
    [FullNameManagement]    NVARCHAR (70) NULL,
    [NicknameManagement]    VARCHAR (30)  NULL,
    [FullNameIncrement]     NVARCHAR (70) NULL,
    [NicknameIncrement]     VARCHAR (30)  NULL,
    [FullNameBVIBonus]      NVARCHAR (70) NULL,
    [NicknameBVIBonus]      VARCHAR (30)  NULL,
    [FullNameLocalBonus]    NVARCHAR (70) NULL,
    [NicknameLocalBonus]    VARCHAR (30)  NULL,
    CONSTRAINT [PK_EmploymentEPA] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);

