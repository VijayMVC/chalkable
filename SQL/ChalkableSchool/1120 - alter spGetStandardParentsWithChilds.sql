alter procedure [dbo].[spGetStandardParentsWithChilds] @standardId int
as
declare @parentsIds table (id int)
declare @children [TStandard]
declare @parentId int = @standardId, @currentStandardId int, @subjectId int
declare @faild bit = 0

insert into @children
select * from [Standard]
where ParentStandardRef = @standardId and IsActive = 1



while @parentId is not null
begin
	select @currentStandardId = Id, @parentId = ParentStandardRef, @subjectId = StandardSubjectRef
	from [Standard]
	where Id = @parentId and IsActive = 1
	
	if @currentStandardId is null
	begin
		set @faild = 1
	end

	if @parentId is not null
	begin 
		insert into @children
		select * from [Standard]
		where ParentStandardRef = @parentId and IsActive = 1
	end
	else 
	begin
		insert into @children
		select * from [Standard]
		where StandardSubjectRef = @subjectId and IsActive = 1
	end

	insert into @parentsIds
	values (@currentStandardId)
end

if @faild = 1
begin
	delete from @parentsIds
	delete from @children
end
select * from @parentsIds
select * from @children

GO


