alter table School 

add IsLEEnabled bit null,
	IsLESyncComplete bit null
	
                   
go

update School
set IsLEEnabled = 0,
	IsLESyncComplete = 0
go

alter table School 
alter column IsLEEnabled bit not null
go

alter table School 
alter column IsLESyncComplete bit not null
go
