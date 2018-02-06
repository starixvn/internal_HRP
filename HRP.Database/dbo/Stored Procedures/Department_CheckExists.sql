-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/31/2018
-- Description:	Check department item exists
-- =============================================
CREATE PROCEDURE [dbo].[Department_CheckExists]
	@DepartmentId SMALLINT = 0
	,@DivisionId SMALLINT = 0 
	,@Code VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@DepartmentId > 0)
	BEGIN
		SELECT TOP 1 @DivisionId= DivisionId
		FROM dbo.Department WITH (NOLOCK) 
		WHERE DepartmentId = @DepartmentId
	END

	SELECT 1
	FROM dbo.Department WITH (NOLOCK)
	WHERE Code = @Code
		AND DivisionId = @DivisionId
		AND DepartmentId <> @DepartmentId
END