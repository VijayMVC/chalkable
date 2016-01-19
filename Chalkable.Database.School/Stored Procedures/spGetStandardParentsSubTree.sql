CREATE Procedure [dbo].[spGetStandardParentsSubTree]
	@CurrentClassId int,		
	@StandardIdForSearch int

As

declare
	@StandartId int = @StandardIdForSearch,
	@IsHiden int,
	@Parent int,
	@CurrentStandardId int,
	@CurrentStandardParent int,
	@Done bit,
	@Column int

declare
	@CurrentClassStandards TStandard
declare
	@ResultTable Table (StandartId int, IsHiden int)
declare
	@StandardTree table 
	(
		Id int, 
		ParentStandardRef int, 
		Name NVARCHAR(100), 
		[Description] NVARCHAR(MAX), 
		StandardSubjectRef int, 
		LowerGradeLevelRef int, 
		UpperGradeLevelRef int, 
		IsActive bit, 
		[Column] int, 
		Done bit, 
		IsSelected bit, 
		AcademicBenchmarkId uniqueidentifier
	)

--	get active Standards for current Class
	insert into @CurrentClassStandards 
	select S.*
			From Class as C
			join ClassStandard as CS on C.Id = CS.ClassRef or CS.ClassRef = C.CourseRef
			join [Standard] as S on S.Id = CS.StandardRef
	where S.IsActive = 1 and C.Id = @CurrentClassId

--	get parent for searched standard and whether it is in current class
	if exists(select * from [Standard] where ParentStandardRef = @StandartId)
	begin
		insert into @ResultTable (StandartId, IsHiden) 
			select S.Id, 
			case 
				when exists(select * from @CurrentClassStandards CC where CC.Id = S.Id) then 0 else 1
			end
			from [Standard] S
			where S.ParentStandardRef = @StandartId
	end

	select	@CurrentStandardId = Id, @CurrentStandardParent = ParentStandardRef from [Standard] where id = @StandartId
--	get all childs for available standards
	while @CurrentStandardParent is not null
	begin
		insert into @ResultTable (StandartId, IsHiden) 
			select S.Id, 
			case 
				when exists(select * from @CurrentClassStandards CC where CC.Id = S.Id) then 0 else 1
			end
			from [Standard] S
			where S.ParentStandardRef = @CurrentStandardParent

			set @StandartId = @CurrentStandardParent
			
			select	@CurrentStandardId = Id, @CurrentStandardParent = ParentStandardRef from [Standard] where id = @StandartId
	end

	set @CurrentStandardId = (select top 1 S.StandardSubjectRef from [Standard] S where id = @StandartId)
--	add root standard
	insert into @ResultTable (StandartId, IsHiden) 
	select S.Id,
			case 
				when exists(select * from @CurrentClassStandards CC where CC.Id = S.Id) then 0 else 1 
			end
			from [Standard] S
			where S.StandardSubjectRef = @CurrentStandardId and S.ParentStandardRef is null

	delete from @ResultTable where IsHiden = 1

	declare ResultTableCursor cursor local fast_forward for
		select * from @ResultTable
	open ResultTableCursor;
	
	fetch next from ResultTableCursor into @StandartId, @IsHiden;
--	add available parent for child
	while @@FETCH_STATUS = 0
	begin
		set @CurrentStandardParent = @StandartId
		while(1=1)
		begin
			if @IsHiden <> -1
			begin
					set @CurrentStandardParent = (select top 1 ParentStandardRef from [Standard] where id = @CurrentStandardParent)
					if @CurrentStandardParent is null 
					begin
						set @Parent = (select StandartId from @ResultTable where IsHiden = -1)
						break
					end 
					if exists(select * from @CurrentClassStandards CC where CC.Id = @CurrentStandardParent)
					begin
						set @Parent = @CurrentStandardParent
						break
					end 
			end
			else
			begin
				set @Parent = @IsHiden
				break
			end
		end

		insert into @StandardTree 
			(	Id, 
				ParentStandardRef, 
				Name, [Description], 
				StandardSubjectRef, 
				LowerGradeLevelRef,
				UpperGradeLevelRef, 
				IsActive, 
				AcademicBenchmarkId, 
				[Column], 
				Done, 
				IsSelected
			) 
			select top 1 CC.*, -1, 0, 0 
			from @CurrentClassStandards CC 
			where CC.Id = @StandartId

		set @Parent = null
		
		fetch next from ResultTableCursor into @StandartId, @IsHiden;
	end

	close ResultTableCursor;
	deallocate ResultTableCursor;

	select top 1 @StandartId = Id, @Parent = ParentStandardRef, @Done = Done from @StandardTree where Done = 0 order by ParentStandardRef ASC
--	calculate column for each standard
	while @StandartId is not null
	begin
		set @Column = null

		if @Parent is null
			set @Column = 0
		else
			set @Column = (select ST.[Column] + 1 from @StandardTree ST where ST.Id = @Parent and ST.Done = 1)
		
		update @StandardTree set Done = 1, [Column] = @Column where Id = @StandartId

		set @StandartId = null

		select top 1 @StandartId = Id, @Parent = ParentStandardRef, @Done = Done from @StandardTree where Done = 0 order by ParentStandardRef ASC
	end

	select top 1 @StandartId = Id, @Parent = ParentStandardRef from @StandardTree where Id = @StandardIdForSearch

	update @StandardTree set IsSelected = 1 where Id = @StandartId;
--	set isSelected mark for UI
	while @Parent is not null
	begin
		update @StandardTree set IsSelected = 1 where Id = @Parent;
		set @Parent = (select top 1 ParentStandardRef from @StandardTree where Id = @Parent)
	end
	
	select 
		ST.Id, 
		ST.ParentStandardRef, 
		ST.Name, 
		ST.[Description], 
		ST.StandardSubjectRef, 
		ST.LowerGradeLevelRef, 
		ST.UpperGradeLevelRef, 
		ST.IsActive, 
		ST.AcademicBenchmarkId, 
		ST.[Column], ST.IsSelected, 
		cast(row_number() over (partition by [Column] order by [Column]) as int) as Row
	from @StandardTree ST
	order by [Column] asc

	GO