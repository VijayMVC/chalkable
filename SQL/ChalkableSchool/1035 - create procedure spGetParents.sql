create procedure spGetParents @studentId uniqueidentifier, @callerId uniqueidentifier, @callerRoleId int
as

declare @studentParents table
(
	Id uniqueidentifier,
	StudentRef uniqueidentifier,
	ParentRef uniqueidentifier
)

insert into @studentParents
select * from StudentParent
where StudentRef = @studentId

select * from @studentParents
if(exists(select * from @studentParents))
begin 
	declare @parentId uniqueidentifier
	declare StudentParentC cursor for
	select  sp.ParentRef from @studentParents sp 

	open StudentParentC
	fetch next from StudentParentC
	into @parentId

	while @@FETCH_STATUS = 0
	begin
		 exec spGetPersonDetails @parentId, @callerId, @callerRoleId
		 fetch next from StudentParentC
		 into @parentId
	end 
	close StudentParentC
	deallocate StudentParentC
end
