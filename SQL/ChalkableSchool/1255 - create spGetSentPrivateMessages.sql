Create Procedure spGetSentMessages
	@personId int,
	@schoolYearId int,
	@roles TInt32 readonly,
	@filter nvarchar(max),
	@start int,
	@count int,
	@classOnly bit
As

select count(*) as AllCount from 
(
	select distinct
		[Id],
		[FromPersonRef],
		[Sent],
		[Subject],
		[Body],
		[DeletedBySender]
	from 
		vwPrivateMessage
	where
		[FromPersonRef] = @personId
		And [DeletedBySender] = 0
		And (not exists(select * from @roles) or RecipientRoleRef in(select * from @roles))
		And (@filter is null 
			 or ([Subject] like(@filter) 
				 or [Body] like(@filter) 
				 or ([Name] is not null and [Name] like(@filter)) 
				 or ([Name] is null and ([RecipientFirstName] like(@filter) or [RecipientLastName] like(@filter)) )
				)
			)
		And (@classOnly = 0 or [RecipientClassRef] is not null)
) as x

declare @MsgIds TInt32;

insert into @MsgIds
select x.[Id] from (select distinct
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender]
from 
	vwPrivateMessage
where
	[FromPersonRef] = @personId
	And [DeletedBySender] = 0
	And (not exists(select * from @roles) or  RecipientRoleRef in(select * from @roles))
	And (@filter is null 
		 Or ([Subject] like(@filter) 
			 or [Body] like(@filter) 
			 or ([Name] is not null and [Name] like(@filter)) 
			 or ([Name] is null and ([RecipientFirstName] like(@filter) or [RecipientLastName] like(@filter)))
			)
		)
	And (@classOnly = 0 or [RecipientClassRef] is not null)
order by [Sent] DESC
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY) as x

select distinct
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender]
From vwPrivateMessage
Where Id in(select * from @MsgIds)

exec spGetRecipients @MsgIds

Go


