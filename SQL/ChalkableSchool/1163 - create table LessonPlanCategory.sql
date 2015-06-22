create table LPGalleryCategory
(
	Id int not null primary key identity(1,1),
	Name nvarchar(255) not null,
	OwnerRef int not null constraint PK_LPGalleryCategory_Person foreign key references Person(Id)
)
Go

alter table LPGalleryCategory
add constraint UQ_Name unique (Name)
Go

