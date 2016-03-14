Alter Table [Application]
add HasTeacherExternalAttach bit null
Go

Alter Table [Application]
add HasStudentExternalAttach bit null
Go

Alter Table [Application]
add HasAdminExternalAttach bit null
Go

update Application
set HasTeacherExternalAttach = 0, HasStudentExternalAttach = 0, HasAdminExternalAttach = 0

Alter Table [Application]
Alter Column HasTeacherExternalAttach bit not null
Go

Alter Table [Application]
Alter Column HasStudentExternalAttach bit not null
Go

Alter Table [Application]
Alter Column HasAdminExternalAttach bit not null
Go


