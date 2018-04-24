
CREATE PROCEDURE [dbo].EmergencyContact_GetByEmployeeId @EmployeeId INT = 0
AS
/*
Created: 20180420@Raymond.Le
Task: Get Emergency Contact by Employee Id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	-- Get EmergencyContact
	SELECT ec.EmployeeId
		,ec.NAME
		,ec.[Address]
		,ec.Relationship
		,ec.PhoneNo1
		,ec.PhoneNo2
	FROM dbo.EmergencyContact AS ec WITH (NOLOCK)
	WHERE ec.EmployeeId = @EmployeeId
END