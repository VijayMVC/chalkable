CREATE TABLE [dbo].[StudentCustomAlertDetail]
(
	Id INT NOT NULL PRIMARY KEY,
    CustomAlertDetailId INT,
    StudentRef INT NOT NULL CONSTRAINT [FK_StudentCustomAlertDetail_Student] FOREIGN KEY REFERENCES [Student] ([Id]),
    SchoolYearRef INT NOT NULL CONSTRAINT [FK_StudentCustomAlertDetail_SchoolYear] FOREIGN KEY REFERENCES [SchoolYear] ([Id]),
    AlertText VARCHAR(500),
    CurrentValue VARCHAR(500)
)

GO	
CREATE NONCLUSTERED INDEX IX_StudentCustomAlertDetail_SchoolYearRef
	ON dbo.StudentCustomAlertDetail( SchoolYearRef )
GO


CREATE NONCLUSTERED INDEX IX_StudentCustomAlertDetail_StudentRef
	ON dbo.StudentCustomAlertDetail( StudentRef )
GO