CREATE TABLE [dbo].[EmploymentWorkPermit] (
    [EmployeeId]     INT      NOT NULL,
    [LocalCompanyId] SMALLINT NULL,
    [Type1]          TINYINT  NULL,
    [StartDateType1] DATE     NULL,
    [EndDateType1]   DATE     NULL,
    [Type2]          TINYINT  NULL,
    [StartDateType2] DATE     NULL,
    [EndDateType2]   DATE     NULL,
    CONSTRAINT [PK_EmploymentWorkPermit] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);



