Create Procedure spGetIncomeMessages
	@personId int,
	@messageId int,
	@roles TInt32 readonly,
	@filter nvarchar(max),
	@read bit,
	@start int,
	@count int
as

Declare @t Table
(
	[Id] int not null,
	[FromPersonRef] int not null,
	[Sent] datetime2,
	[Subject] nvarchar(255),
	[Body] nvarchar(255),
	[DeletedBySender] bit,
	[Read] bit,
	[DeletedByRecipient] bit,
	SenderId int not null,
	SenderFirstName nvarchar(255),
	SenderLastName nvarchar(255),
	SenderSalutation nvarchar(255),
	SenderRoleRef int,
	SenderGender nvarchar(255)
)

insert into @t
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
    SenderSalutation,
    SenderRoleRef,
    SenderGender
from 
	vwIncomeMessage
where 
	RecipientRef = @personId
	And DeletedByRecipient = 0
	And (@read is null or [Read] = @read)
	And (@messageId is null or Id = @messageId)
	And (not exists(select * from @roles) or SenderRoleRef in(select * from @roles))
	And (@filter is null or 
		[Subject] like(@filter) 
		or [Body] like(@filter)
		or SenderFirstName like(@filter) 
		or SenderLastName like(@filter))

select count(*) as AllCount from @t

select * from @t
Order By 
	[Sent] Desc 
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY 

Go