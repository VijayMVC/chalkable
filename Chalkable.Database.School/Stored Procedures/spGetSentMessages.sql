CREATE Procedure [dbo].[spGetSentMessages]
@personId int,
@roles TInt32 readonly,
@filter nvarchar(max),
@start int,
@count int,
@classOnly bit,
@fromDate datetime2,
@toDate datetime2
As

declare @rl bit = (select count(*) from @roles)

select
	count(distinct pm.[Id]) as AllCount
from
	PrivateMessage pm
	join PrivateMessageRecipient pmr
	on pm.Id = pmr.PrivateMessageRef
	join 
	(
		Select
			[Id],
			[FirstName] as RecipientFirstName,
			[LastName] as RecipientLastName,
			3 as RecipientRoleRef
		From
			Student
		Union
		Select
			[Id],
			[FirstName] as RecipientFirstName,
			[LastName] as RecipientLastName,
			2 as RecipientRoleRef
		From
			Staff
	) as Recipient
	on Recipient.Id = pmr.RecipientRef
	left join 
	(
		Select
			[Id],
			[Name] as [ClassName]
		From
			Class
	) as RecipientClass
	on RecipientClass.Id = pmr.RecipientClassRef
	where
		[FromPersonRef] = @personId
		And [DeletedBySender] = 0
		And (@rl = 0 or (RecipientRoleRef in(select * from @roles) And RecipientClassRef is null))
		And 
		(
			@filter is null
			or 
			(
				[Subject] like(@filter)
				or [Body] like(@filter)
				or ([ClassName] is not null and [ClassName] like(@filter))
				or ([ClassName] is null and ([RecipientFirstName] like(@filter) or [RecipientLastName] like(@filter)))
			)
		)
		And (@classOnly = 0 Or [RecipientClassRef] is not null)
		And (@fromDate is null or @fromDate<=[Sent])
		And (@toDate is null or [Sent]<=@toDate)

declare @SentMsg table
(
	[Id] int,
	[FromPersonRef] int,
	[Sent] datetime2,
	[Subject] nvarchar(max),
	[Body] nvarchar(max),
	[DeletedBySender] bit
)

declare @MsgIds TInt32;

insert into @SentMsg
select
	x.[Id],
	x.FromPersonRef,
	x.[Sent],
	x.[Subject],
	x.Body,
	x.DeletedBySender
from 
(
	select
		pm.[Id],
		[FromPersonRef],
		[Sent],
		[Subject],
		[Body],
		[DeletedBySender]
	from
		PrivateMessage pm
		join PrivateMessageRecipient pmr
	on pm.Id = pmr.PrivateMessageRef
	join 
	(
		Select
		[Id],
		[FirstName] as RecipientFirstName,
		[LastName] as RecipientLastName,
		3 as RecipientRoleRef
		From
		Student
		Union
		Select
		[Id],
		[FirstName] as RecipientFirstName,
		[LastName] as RecipientLastName,
		2 as RecipientRoleRef
		From
		Staff
	) as Recipient
	on Recipient.Id = pmr.RecipientRef
	left join 
	(
		Select
		[Id],
		[Name] as [ClassName]
	From
		Class
	) as RecipientClass
	on RecipientClass.Id = pmr.RecipientClassRef
	where
		[FromPersonRef] = @personId
		And [DeletedBySender] = 0
		And (@rl = 0 or (RecipientRoleRef in(select * from @roles) And RecipientClassRef is null))
		And 
		(
			@filter is null
			or 
			(
				[Subject] like(@filter)
				or [Body] like(@filter)
				or ([ClassName] is not null and [ClassName] like(@filter))
				or ([ClassName] is null and ([RecipientFirstName] like(@filter) or [RecipientLastName] like(@filter)) )
			)
		)
		And (@classOnly = 0 Or [RecipientClassRef] is not null)
		And (@fromDate is null or @fromDate<=[Sent])
		And (@toDate is null or [Sent]<=@toDate)
		group by
			pm.[Id],
			[FromPersonRef],
			[Sent],
			[Subject],
			[Body],
			[DeletedBySender]
		order by [Sent] DESC

		OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
) as x

select distinct
	pm.[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender]
From
	@SentMsg pm
Order By [Sent] Desc


insert into @MsgIds
select [Id] from @SentMsg

exec spGetMessageRecipients @MsgIds
GO