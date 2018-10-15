CREATE TABLE [dbo].[EmergencyContact] (
    [EmployeeId]   INT            NOT NULL,
    [Name]         NVARCHAR (70)  NULL,
    [Relationship] NVARCHAR (70)  NULL,
    [Address]      NVARCHAR (500) NULL,
    [PhoneNo1]     NVARCHAR (30)  NULL,
    [PhoneNo2]     NVARCHAR (30)  NULL,
    CONSTRAINT [PK_EmergencyContact] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);



