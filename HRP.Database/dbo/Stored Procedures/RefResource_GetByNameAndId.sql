-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/08/2018
-- Description:	Get RefResource by id and Name
-- =============================================
Create PROCEDURE [dbo].RefResource_GetByNameAndId 
	@ResourceId SMALLINT = NULL
	,@RefName VARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ResourceId
		,RefName
		,Value
		,[Description]
		,IsActive
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = @RefName
	AND ResourceId = @ResourceId

END