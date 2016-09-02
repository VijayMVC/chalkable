CREATE Procedure spBeforeSync
As
	ALTER TABLE [Standard] NOCHECK CONSTRAINT FK_Standard_Standard;
	ALTER TABLE [Topic] NOCHECK CONSTRAINT FK_Topic_Topic;  

	Delete From [StandardDerivative]

	Delete From Authority

	Delete From Course

	Delete From Document

	Delete From GradeLevel

	Delete From [Subject]

	Delete From SubjectDoc