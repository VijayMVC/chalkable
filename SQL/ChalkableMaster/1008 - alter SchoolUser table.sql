drop table SchoolUser
go
drop type TSchoolUser
go

create Table SchoolUser
(
	SchoolRef int not null,
	UserRef int not null,
	DistrictRef uniqueidentifier not null constraint FK_SchoolUser_District foreign key references District(Id)
)
go

alter table SchoolUser
add constraint PK_SchoolUser primary key (SchoolRef, UserRef, DistrictRef)
go

create type TSchoolUser as table 
(
	SchoolRef int not null,
	UserRef int not null,
	DistrictRef uniqueidentifier not null	
)
go
