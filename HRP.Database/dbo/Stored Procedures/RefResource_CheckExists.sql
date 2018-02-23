-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/08/2018
-- Description:	Check RefResource item existed
-- =============================================
CREATE PROCEDURE [dbo].[RefResource_CheckExists]
	@ResourceId SMALLINT = 0
	,@RefName VARCHAR(100)
	,@Value NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = @RefName
		AND Value = @Value
		AND ResourceId <> @ResourceId
END