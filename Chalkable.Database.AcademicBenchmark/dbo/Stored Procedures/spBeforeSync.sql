CREATE Procedure [dbo].[spBeforeSync]
As
--Can fail if parent hasn't downloaded yet
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_Standard;
--Need this in order to delete all data from tables.
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_Authority;
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_Course;
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_Document;
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_GradeLevel_Hi;
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_GradeLevel_Lo;
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_Subject;
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_SubjectDoc;

	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_Topic;
	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_Course;  
	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_GradeLevel_Hi;
	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_GradeLevel_Lo; 
	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_Subject;
	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_SubjectDoc;

	--Currentlly we don't do anything with this table.
	--No Sync. Just Import. Before full import, this table
	--should be empty.
	--Delete From [StandardDerivative]
	Delete From Authority
	Delete From Course
	Delete From Document
	Delete From GradeLevel
	Delete From [Subject]
	Delete From SubjectDoc