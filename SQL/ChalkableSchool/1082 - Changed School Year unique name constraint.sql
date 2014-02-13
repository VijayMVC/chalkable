alter table SchoolYear 
	drop QU_SchoolYear_Name
GO
alter table SchoolYear 
	add constraint UQ_SchoolYear_Name unique (SchoolRef, Name)
GO