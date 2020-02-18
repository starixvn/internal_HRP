CREATE TABLE [dbo].[Employee] (
    [EmployeeId]       INT            IDENTITY (1, 1) NOT NULL,
    [Nickname]         VARCHAR (30)   NOT NULL,
    [FullName]         NVARCHAR (70)  NOT NULL,
    [Gender]           TINYINT        NULL,
    [DateOfBirth]      DATE           NULL,
    [PlaceOfBirth]     NVARCHAR (100) NULL,
    [Nationality]      SMALLINT       NULL,
    [NRICNo]           VARCHAR (20)   NULL,
    [MaritalStatus]    TINYINT        NULL,
    [NoOfChildren]     TINYINT        NULL,
    [OverseasPhoneNo]  VARCHAR (15)   NULL,
    [LocalPhoneNo]     VARCHAR (15)   NULL,
    [OtherPhoneNo]     VARCHAR (15)   NULL,
    [PassportNo]       VARCHAR (15)   NULL,
    [DateIssue]        DATE           NULL,
    [ExpiredDate]      DATE           NULL,
    [PlaceOfIssue]     NVARCHAR (100) NULL,
    [Skype]            VARCHAR (32)   NULL,
    [PersonalEmail]    VARCHAR (254)  NULL,
    [PermanentAddress] NVARCHAR (500) NULL,
    [ResidenceAddress] NVARCHAR (500) NULL,
    [MailingAddress]   NVARCHAR (500) NULL,
    [ProfilePic]       VARCHAR (200)  NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);



