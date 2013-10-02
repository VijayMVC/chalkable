sp_RENAME 'Fund.[Descrption]' , 'Description', 'COLUMN'
go


alter table Fund
drop column FundRequestRef 
go

alter table Fund
add FundRequestRef uniqueidentifier null constraint FK_Fund_FundRequest foreign key references FundRequest(Id)
go
