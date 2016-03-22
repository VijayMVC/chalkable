Alter Table School
Add StudentMessagingEnabled bit  null
Go

Alter Table School
Add	StudentToClassMessagingOnly bit  null
Go

Alter Table School
Add TeacherToStudentMessaginEnabled bit  null
Go

Alter Table School
Add TeacherToClassMessagingOnly bit  null
Go

Update School
Set StudentMessagingEnabled = 1, StudentToClassMessagingOnly = 0, TeacherToStudentMessaginEnabled = 1, TeacherToClassMessagingOnly = 0
Where IsMessagingDisabled = 0

Update School
Set StudentMessagingEnabled = 0, StudentToClassMessagingOnly = 0, TeacherToStudentMessaginEnabled = 0, TeacherToClassMessagingOnly = 0
Where IsMessagingDisabled = 1
Go


Alter Table School
Alter Column StudentMessagingEnabled bit not null
Go

Alter Table School
Alter Column StudentToClassMessagingOnly bit not null
Go

Alter Table School
Alter Column TeacherToStudentMessaginEnabled bit not null
Go

Alter Table School
Alter Column TeacherToClassMessagingOnly bit not null
Go
