Alter Procedure [dbo].[spGetSentMessages]
	@personId int,
	@schoolYearId int,
	@roles TInt32 readonly,
	@filter nvarchar(max),
	@start int,
	@count int,
	@classOnly bit
As

declare @rl bit = (select count(*) from @roles)

select 
	count(distinct pm.[Id]) as AllCount
from 
	PrivateMessage pm
	join PrivateMessageRecipient pmr
		on pm.Id = pmr.PrivateMessageRef
	join (Select
				[Id], 
				[FirstName] as RecipientFirstName, 
				[LastName] as RecipientLastName,
				3 as RecipientRoleRef
		  From
				Person
		  Union
		  Select
				[Id],
				[FirstName] as RecipientFirstName,
				[LastName] as RecipientLastName,
				2 as RecipientRoleRef
		  From
				Staff) as Recipient
		on Recipient.Id = pmr.RecipientRef
	left join (Select
				[Id],
				[Name] as [ClassName]
		  From
				Class) as RecipientClass
		on RecipientClass.Id = pmr.RecipientClassRef
where
	[FromPersonRef] = @personId
	And [DeletedBySender] = 0
	And (@rl = 0 or RecipientRoleRef in(select * from @roles))
	And (@filter is null 
		 or ([Subject] like(@filter) 
			 or [Body] like(@filter) 
			 or ([ClassName] is not null and [ClassName] like(@filter)) 
			 or ([ClassName] is null and ([RecipientFirstName] like(@filter) or [RecipientLastName] like(@filter)) )
			)
		)
	And (@classOnly = 0 or [RecipientClassRef] is not null)
	And pm.SchoolYearRef = @schoolYearId

declare @MsgIds TInt32;

insert into @MsgIds 
select x.[Id] from (
	select
		pm.[Id]
	from 
		PrivateMessage pm
		join PrivateMessageRecipient pmr
			on pm.Id = pmr.PrivateMessageRef
		join (Select
					[Id], 
					[FirstName] as RecipientFirstName, 
					[LastName] as RecipientLastName,
					3 as RecipientRoleRef
			  From
					Person) as Recipient
			on Recipient.Id = pmr.RecipientRef
		left join (Select
					[Id],
					[Name] as [ClassName]
			  From
					Class) as RecipientClass
			on RecipientClass.Id = pmr.RecipientClassRef
	where
		[FromPersonRef] = @personId
		And [DeletedBySender] = 0
		And (@rl = 0 or RecipientRoleRef in(select * from @roles))
		And (@filter is null 
			 or ([Subject] like(@filter) 
				 or [Body] like(@filter) 
				 or ([ClassName] is not null and [ClassName] like(@filter)) 
				 or ([ClassName] is null and ([RecipientFirstName] like(@filter) or [RecipientLastName] like(@filter)) )
				)
			)
		And (@classOnly = 0 or [RecipientClassRef] is not null)
		And pm.SchoolYearRef = @schoolYearId
	group by 
		pm.Id, 
		[Sent]
	order by [Sent] DESC

	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
) as x

select distinct
	pm.[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[SchoolYearRef]
From 
	PrivateMessage pm
Where pm.Id in(select * from @MsgIds)

exec spGetMessageRecipients @MsgIds


GO