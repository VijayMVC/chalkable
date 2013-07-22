alter table Developer
add constraint UQ_Developer_SchoolRef unique (SchoolRef)
go
alter table ApplicationGradeLevel
add constraint UQ_ApplicationGradeLevel_ApplicationRef_GradeLevel unique(ApplicationRef, GradeLevel)
go
alter table ApplicationPermission
add constraint UQ_ApplicationPermission_ApplicationRef_Permission unique(ApplicationRef, Permission)
go
