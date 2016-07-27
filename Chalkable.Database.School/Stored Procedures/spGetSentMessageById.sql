

Create Procedure [dbo].[spGetSentMessageById]
@personId int,
@messageId int
As

select top 1
[Id],
[FromPersonRef],
[Sent],
[Subject],
[Body],
[DeletedBySender]
from
PrivateMessage
where
[FromPersonRef] = @personId
And [DeletedBySender] = 0
And Id = @messageId

declare @tb TInt32
insert into @tb
values (@messageId)

exec spGetMessageRecipients @tb