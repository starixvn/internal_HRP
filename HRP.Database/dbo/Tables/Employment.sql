CREATE TABLE [dbo].[Employment] (
    [EmployeeId]         INT            NOT NULL,
    [CompanyId]          SMALLINT       NOT NULL,
    [LocalCompanyId]     SMALLINT       NOT NULL,
    [JobTitle]           NVARCHAR (100) NOT NULL,
    [LocalJobTitleId]    SMALLINT       NOT NULL,
    [EmployeeCode]       VARCHAR (15)   NOT NULL,
    [EmploymentType]     TINYINT        NOT NULL,
    [EmploymentStatus]   TINYINT        NOT NULL,
    [JobCategoryId]      SMALLINT       NOT NULL,
    [JobGradeId]         SMALLINT       NOT NULL,
    [GradeLevelId]       SMALLINT       NOT NULL,
    [DivisionId]         SMALLINT       NOT NULL,
    [DepartmentId]       SMALLINT       NOT NULL,
    [SectionId]          SMALLINT       NOT NULL,
    [CostCenterId]       SMALLINT       NOT NULL,
    [GroupStartDate]     DATE           NOT NULL,
    [GroupEndDate]       DATE           NULL,
    [CompanyStartDate]   DATE           NULL,
    [CompanyEndDate]     DATE           NULL,
    [Email]              VARCHAR (254)  NOT NULL,
    [CountryId]          SMALLINT       NOT NULL,
    [CityId]             SMALLINT       NOT NULL,
    [OfficeId]           SMALLINT       NOT NULL,
    [DST]                TINYINT        NULL,
    [DBT]                TINYINT        NULL,
    [TaxIDNo]            VARCHAR (50)   NULL,
    [SSSNo]              VARCHAR (50)   NULL,
    [HealthInsNo]        VARCHAR (50)   NULL,
    [HDMFNo]             VARCHAR (50)   NULL,
    [PDFPassword]        VARCHAR (1000) NOT NULL,
    [FullNamePFileAdmin] NVARCHAR (70)  NOT NULL,
    [NicknamePFileAdmin] VARCHAR (30)   NULL,
    CONSTRAINT [PK_Employment] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);







