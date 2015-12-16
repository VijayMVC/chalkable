Alter Table SchoolYear
Add AcadYear int null
Go
update SchoolYear 
Set AcadYear = DatePart(yyyy, getdate())
Go
Alter Table SchoolYear
Alter Column AcadYear int not null
Go

