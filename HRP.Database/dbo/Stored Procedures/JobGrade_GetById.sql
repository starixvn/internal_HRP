
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/06/2018
-- Description:	Get JobGrade By Id
-- =============================================
Create PROCEDURE [dbo].JobGrade_GetById @JobGradeId SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT jg.JobGradeId
		,jg.[Name]
		,jg.[Description]
		,jg.IsActive
		,jg.GradeLevelId
		,g.[Name] AS GradeLevelName
		,jc.JobCategoryId
		,jc.Name AS JobCategoryName
	FROM dbo.JobGrade AS jg WITH (NOLOCK) 
	LEFT JOIN dbo.GradeLevel AS g WITH (NOLOCK) ON g.GradeLevelId = jg.GradeLevelId
	LEFT JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON g.JobCategoryId = jc.JobCategoryId
	WHERE jg.JobGradeId = @JobGradeId
END