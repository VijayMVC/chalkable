alter table [User] 
add IsDistrictRegistrator bit null
go


update [User]
set IsDistrictRegistrator = 0
where Login <> 'InformationNow@Chalkable.com'
go

update [User]
set IsDistrictRegistrator = 1,
	IsSysAdmin = 0
where Login = 'InformationNow@Chalkable.com'
go

alter table [User] 
alter column IsDistrictRegistrator bit not null
go