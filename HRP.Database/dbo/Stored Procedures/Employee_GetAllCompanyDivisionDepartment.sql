-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Employee_GetAllCompanyDivisionDepartment] 
	@UserId INT
AS
BEGIN
	
	SET NOCOUNT ON;
	--Get Company
	SELECT c.CompanyId
		,c.Code
		,c.Name
	FROM dbo.Company AS c WITH (NOLOCK)
		inner join dbo.UserFunction AS Uf WITH (NOLOCK) ON c.CompanyId = Uf.CompanyId
	WHERE Uf.UserId = @UserId
		AND Uf.FunctionId = 5
		AND c.IsActive = 1
		AND Uf.IsRead = 1

		--Get division
	SELECT Di.DivisionId
		,Di.Code
		,Di.CompanyId
		,Di.Name
	FROM dbo.Division AS Di WITH (NOLOCK)
		inner join dbo.UserFunction AS Uf WITH (NOLOCK) ON Di.CompanyId = Uf.CompanyId
	WHERE Uf.UserId = @UserId
		AND Uf.FunctionId = 5
		AND Di.IsActive = 1
		AND Uf.IsRead = 1

		--Get department
	SELECT De.DepartmentId
		,De.Code
		,De.DivisionId
		,De.Name
	FROM dbo.Department AS De WITH (NOLOCK)
		inner join dbo.Division AS Di WITH (NOLOCK) ON De.DivisionId = Di.DivisionId 
		inner join dbo.UserFunction AS Uf WITH (NOLOCK) ON Di.CompanyId = Uf.CompanyId
	WHERE Uf.UserId = @UserId
		AND Uf.FunctionId = 5
		AND Di.IsActive = 1 AND De.IsActive = 1
		AND Uf.IsRead = 1
END