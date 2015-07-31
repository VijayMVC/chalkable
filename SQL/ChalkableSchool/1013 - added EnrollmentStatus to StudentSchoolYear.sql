alter table StudentSchoolYear
add EnrollmentStatus int null 
go

update StudentSchoolYear
set EnrollmentStatus = 0
go

alter table StudentSchoolYear
alter column EnrollmentStatus int not null 
go

