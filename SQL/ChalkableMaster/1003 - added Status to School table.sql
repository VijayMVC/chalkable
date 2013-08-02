alter table School
add [Status] int null
go

update School
set [Status] = 1
go

alter table School
alter column [Status] int not null 
go