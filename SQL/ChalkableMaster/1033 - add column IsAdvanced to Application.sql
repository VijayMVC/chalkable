alter table Application 
add IsAdvanced bit null
go

update Application
set IsAdvanced = 1
go

alter table Application 
alter column IsAdvanced bit not null
go