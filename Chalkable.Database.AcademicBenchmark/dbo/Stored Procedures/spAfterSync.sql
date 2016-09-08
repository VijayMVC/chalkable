

CREATE Procedure [dbo].[spAfterSync]
As
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_Standard;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_Authority;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_Course;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_Document;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_GradeLevel_Hi;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_GradeLevel_Lo;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_Subject;
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_SubjectDoc;

	ALTER TABLE [Topic] CHECK CONSTRAINT FK_Topic_Topic;
	ALTER TABLE [Topic] CHECK CONSTRAINT FK_Topic_Course;  
	ALTER TABLE [Topic] CHECK CONSTRAINT FK_Topic_GradeLevel_Hi;
	ALTER TABLE [Topic] CHECK CONSTRAINT FK_Topic_GradeLevel_Lo; 
	ALTER TABLE [Topic] CHECK CONSTRAINT FK_Topic_Subject;
	ALTER TABLE [Topic] CHECK CONSTRAINT FK_Topic_SubjectDoc;