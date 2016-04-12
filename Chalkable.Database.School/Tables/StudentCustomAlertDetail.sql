CREATE TABLE [dbo].[StudentCustomAlertDetail]
(
	Id INT NOT NULL PRIMARY KEY,
    CustomAlertDetailId INT,
    StudentRef INT NOT NULL CONSTRAINT [FK_StudentRef_Student] FOREIGN KEY REFERENCES [Student] ([Id]),
    SchoolYearRef INT NOT NULL CONSTRAINT [FK_SchoolYearRef_SchoolYear] FOREIGN KEY REFERENCES [SchoolYear] ([Id]),
    AlertText VARCHAR(500),
    CurrentValue VARCHAR(500)
)
