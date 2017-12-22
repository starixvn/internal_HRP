SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Menu_GetByUserId] @UserId INT
AS
/*
Created: 20171201@Tim.Trinh
Task: Get menu by UserId.
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT menu.MenuId
		,menu.[Name]
		,menu.[Description]
		,menu.URL
		,menu.ParentId
		,menu.FunctionId
		,menu.DisplayOrder
		,menu.IsActive
	FROM dbo.Menu AS menu WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT f.FunctionId
		FROM dbo.[Function] AS f WITH (NOLOCK)
			INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON f.FunctionId = uf.FunctionId
			INNER JOIN dbo.[User] AS u WITH (NOLOCK) ON uf.UserId = u.UserId
			AND u.IsActive = 1
		WHERE u.UserId = @UserId
		) AS f ON menu.FunctionId = f.FunctionId
END

GO


