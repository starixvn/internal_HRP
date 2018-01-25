CREATE TABLE [dbo].[EmploymentWorkPermit] (
    [EmployeeId]     INT      NOT NULL,
    [LocalCompanyId] SMALLINT NOT NULL,
    [Type1]          TINYINT  NOT NULL,
    [StartDateType1] DATE     NOT NULL,
    [EndDateType1]   DATE     NOT NULL,
    [Type2]          TINYINT  NULL,
    [StartDateType2] DATE     NULL,
    [EndDateType2]   DATE     NULL,
    CONSTRAINT [PK_EmploymentWorkPermit] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);

