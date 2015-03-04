alter table [Application]
add UpdateDateTime datetime2
go

update [Application]  
set [Application].UpdateDateTime = [Application].createDateTime  
from [Application]

alter table [Application]
alter column [UpdateDateTime] datetime2 null
go

