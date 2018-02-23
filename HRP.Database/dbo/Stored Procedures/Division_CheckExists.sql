
CREATE PROCEDURE [dbo].[Division_CheckExists]
	@DivisionId SMALLINT = 0
	,@CompanyId SMALLINT = 0 
	,@Code NVARCHAR(50)
AS
/*
Created: 20180131@Raymond.Le
Task: Check division item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (@DivisionId > 0)
	BEGIN
		SELECT TOP 1 @CompanyId= CompanyId 
		FROM dbo.Division WITH (NOLOCK) 
		WHERE DivisionId = @DivisionId
	END

	SELECT 1
	FROM dbo.Division WITH (NOLOCK)
	WHERE Code = @Code
		AND CompanyId = @CompanyId
		AND DivisionId <> @DivisionId
END