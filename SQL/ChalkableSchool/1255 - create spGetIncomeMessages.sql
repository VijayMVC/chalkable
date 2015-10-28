Create Procedure [dbo].[spGetIncomeMessages]
	@personId int,
	@roles TInt32 readonly,
	@filter nvarchar(max),
	@read bit,
	@start int,
	@count int
as

select count(*) as AllCount from 
(select
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[Read],
	[DeletedByRecipient],
	SenderId,
    SenderFirstName,
    SenderLastName,
    SenderRoleRef,
    SenderGender
from 
	vwIncomeMessage
where 
	RecipientRef = @personId
	And DeletedByRecipient = 0
	And (@read is null or [Read] = @read)
	And (not exists(select * from @roles) or SenderRoleRef in(select * from @roles))
	And (@filter is null or 
		[Subject] like(@filter) 
		or [Body] like(@filter)
		or SenderFirstName like(@filter) 
		or SenderLastName like(@filter)))as x

select
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[Read],
	[DeletedByRecipient],
	SenderId,
    SenderFirstName,
    SenderLastName,
    SenderRoleRef,
    SenderGender
from 
	vwIncomeMessage
where 
	RecipientRef = @personId
	And DeletedByRecipient = 0
	And (@read is null or [Read] = @read)
	And (not exists(select * from @roles) or SenderRoleRef in(select * from @roles))
	And (@filter is null or 
		[Subject] like(@filter) 
		or [Body] like(@filter)
		or SenderFirstName like(@filter) 
		or SenderLastName like(@filter))
Order By 
	[Sent] Desc 
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY 



GO


