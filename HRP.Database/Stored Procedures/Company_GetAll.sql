SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Company_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20171205@Tim.Trinh
Task: Get all function permission
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CompanyId
		,[Name]
		,IsActive
	FROM dbo.[Company] WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
END

GO


