CREATE TYPE [dbo].[TStudentCustomAlertDetail] AS TABLE(
	Id INT NOT NULL,
    CustomAlertDetailId INT,
    StudentRef INT NOT NULL,
    SchoolYearRef INT NOT NULL,
    AlertText VARCHAR(500),
    CurrentValue VARCHAR(500)
)
