Alter Procedure [dbo].[spGetSentMessageById]
	@personId int,
	@schoolYearId int,
	@messageId int
As

select top 1
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[SchoolYearRef]
from 
	PrivateMessage
where
	[FromPersonRef] = @personId
	And [DeletedBySender] = 0
	And Id = @messageId
	And SchoolYearRef = @schoolYearId

declare @tb TInt32
insert into @tb
values (@messageId)

exec spGetMessageRecipients @tb


GO


