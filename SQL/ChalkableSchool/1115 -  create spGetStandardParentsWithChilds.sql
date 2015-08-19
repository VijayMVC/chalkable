Create procedure spGetStandardParentsWithChilds @standardId int
as
declare @parentsIds table (id int)
declare @parentChildsIds [TStandard]
declare @parentId int = @standardId, @currentStandardId int, @subjectId int
while @parentId is not null
begin
	select @currentStandardId = Id, @parentId = ParentStandardRef, @subjectId = StandardSubjectRef
	from [Standard]
	where Id = @parentId
	
	if @parentId is not null
	begin 
		insert into @parentChildsIds
		select * from [Standard]
		where ParentStandardRef = @parentId
	end
	else 
	begin
		insert into @parentChildsIds
		select * from [Standard]
		where StandardSubjectRef = @subjectId
	end

	insert into @parentsIds
	values (@currentStandardId)
end
select * from @parentsIds
select * from @parentChildsIds
Go
