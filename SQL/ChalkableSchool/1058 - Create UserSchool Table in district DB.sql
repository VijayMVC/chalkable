Create Table UserSchool
(
	UserRef int not null,
	SchoolRef int not null Constraint FK_UserSchool_School Foreign Key References School(Id)
	Primary Key (UserRef, SchoolRef)
)
GO
Create Type TUserSchool as Table
(
	UserRef int not null,
	SchoolRef int not null 
)
GO
Create Index IX_UserSchool_User on UserSchool(UserRef)
GO
Create Index IX_UserSchool_School on UserSchool(SchoolRef)
GO