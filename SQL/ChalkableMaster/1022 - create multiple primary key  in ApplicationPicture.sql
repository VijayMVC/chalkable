declare @applicationPicture table
(
	Id uniqueidentifier,
	ApplicationRef uniqueidentifier 
)

insert into @applicationPicture
select * from ApplicationPicture

drop table ApplicationPicture

create table ApplicationPicture
(
	Id uniqueidentifier not null,
	ApplicationRef uniqueidentifier not null constraint FK_ApplicationPicture_Application foreign key references [Application](Id)
)
alter table ApplicationPicture 
add constraint PK_ApplicationPicture_Id_ApplicationRef primary key (Id, ApplicationRef)

insert into ApplicationPicture
select * from @applicationPicture
















