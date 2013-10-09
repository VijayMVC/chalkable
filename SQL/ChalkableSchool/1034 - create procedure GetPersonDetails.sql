create procedure spGetPersonDetails @personId uniqueidentifier, @callerId uniqueidentifier, @callerRoleId int
as

exec spGetPersons @personId, @callerId, null, 0, 1, null, null, null, null,null,null,null,0, @callerRoleId

select * from [Address]
where PersonRef = @personId

select * from Phone
where PersonRef = @personId

go