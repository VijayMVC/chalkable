alter table Person
drop column	[SisStudentUserId] 
go
alter table Person
drop column [SisStaffUserId]
go

alter table Person
add UserId int null
go

