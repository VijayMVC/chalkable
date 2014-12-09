CREATE NONCLUSTERED INDEX 
	[IX_ClassTeacher_Class] ON [dbo].[ClassTeacher]([ClassRef])
GO


CREATE NONCLUSTERED INDEX 
	[IX_ClassTeacher_Teracher] ON [dbo].[ClassTeacher]([PersonRef])
GO