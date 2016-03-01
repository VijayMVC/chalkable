Alter PROCEDURE [dbo].[spGetNotInstalledStudentsCountPerApplication]
	 @staffId INT, 
	 @classId INT, 
	 @markingPeriodId INT
AS

DECLARE @schoolYearId INT 

SELECT @schoolYearId = SchoolYearRef 
FROM MarkingPeriod 
WHERE Id = @markingPeriodId;

WITH Applications AS(
	SELECT 
		ApplicationRef 
	FROM 
		ApplicationInstall
	WHERE 
		PersonRef = @staffId 
		AND Active = 1 
		AND SchoolYearRef = @schoolYearId
	GROUP BY ApplicationRef
),
Roster AS (
	SELECT 
		PersonRef 
	FROM 
		ClassPerson cp
		JOIN StudentSchoolYear ssy ON ssy.StudentRef = cp.PersonRef
	WHERE 
		cp.ClassRef = @classId
		AND cp.MarkingPeriodRef = @markingPeriodId
		AND cp.IsEnrolled = 1
		AND ssy.EnrollmentStatus = 0
		AND ssy.SchoolYearRef = @schoolYearId
	GROUP BY cp.PersonRef
),
ApplicationsCount AS (
	SELECT
		a.ApplicationRef,
		NotInstalledStudentCount = SUM(CASE WHEN ai.Id IS NULL THEN 1 ELSE 0 END)
	FROM
		Applications a
		CROSS JOIN Roster r
		LEFT JOIN ApplicationInstall ai ON ai.ApplicationRef = a.ApplicationRef AND ai.PersonRef = r.PersonRef
		AND ai.Active = 1 
		AND ai.SchoolYearRef= @SchoolYearId
	GROUP By a.ApplicationRef
)
SELECT 
	ApplicationId = a.ApplicationRef,
	NotInstalledStudentCount = COALESCE(NotInstalledStudentCount, 0)
FROM 
	Applications a
	LEFT JOIN ApplicationsCount ac ON ac.ApplicationRef = a.ApplicationRef
GO


