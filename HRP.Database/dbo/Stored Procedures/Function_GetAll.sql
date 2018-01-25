
CREATE PROCEDURE [dbo].[Function_GetAll] @IsActive BIT = NULL
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
	SET NOCOUNT ON

	SELECT f.FunctionId
		,f.[Name]
		,f.FunctionKey
		,f.IsReadOnly
		,f.ParentId
		,f.IsActive
		,f.IsAdmin
	FROM dbo.[Function] AS f WITH (NOLOCK)
	WHERE ISNULL(@IsActive, f.IsActive) = f.IsActive
END