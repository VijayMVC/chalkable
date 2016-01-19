CREATE Procedure [dbo].[spGetStandardParentsSubTree]
	@CurrentClassId int,		
	@StandardIdForSearch int

As

declare
	@StandartId int = @StandardIdForSearch,
	@IsHiden int,
	@Parrent int,
	@CurrentStandardId int,
	@CurrentStandardParent int,
	@Done bit,
	@Column int,
	@Name NVARCHAR(100), 
	@Description NVARCHAR(MAX), 
	@StandardSubjectRef int,
	@LowerGradeLevelRef int, 
	@UpperGradeLevelRef int, 
	@IsActive bit,
	@AcademicBenchmarkId uniqueidentifier

declare
	@CurrentClassStandards TStandard
declare
	@ResultTable Table (StandartId int, IsHiden int)
declare
	@StandardTree table (Id int, ParentStandardRef int, Name NVARCHAR(100), [Description] NVARCHAR(MAX), StandardSubjectRef int, LowerGradeLevelRef int, UpperGradeLevelRef int, IsActive bit, [Column] int, Done bit, IsSelected bit, AcademicBenchmarkId uniqueidentifier)

	insert into @CurrentClassStandards 
	select S.*
			From Class as C
			join ClassStandard as CS on C.Id = CS.ClassRef or CS.ClassRef = C.CourseRef
			join [Standard] as S on S.Id = CS.StandardRef
	where S.IsActive = 1 and C.Id = @CurrentClassId


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

	while @CurrentStandardParent is not null
	begin
		insert into @ResultTable (StandartId, IsHiden) 
			select S.Id, 
			case 
				when exists(select * from @CurrentClassStandards CC where CC.Id = S.Id) then 0 else 1
			end
			from [Standard] S
			where S.ParentStandardRef = @CurrentStandardParent

			select @StandartId = @CurrentStandardParent
			
			select	@CurrentStandardId = Id, @CurrentStandardParent = ParentStandardRef from [Standard] where id = @StandartId
	end

	select top 1 @CurrentStandardId = S.StandardSubjectRef from [Standard] S where id = @StandartId

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

	while @@FETCH_STATUS = 0
	begin
		select @CurrentStandardParent = @StandartId
		while(1=1)
		begin
			if @IsHiden <> -1
			begin
					select @CurrentStandardParent = ParentStandardRef from [Standard] where id = @CurrentStandardParent
					if @CurrentStandardParent is null 
					begin
						select @Parrent = StandartId from @ResultTable where IsHiden = -1
						break
					end 
					if exists(select * from @CurrentClassStandards CC where CC.Id = @CurrentStandardParent)
					begin
						select @Parrent = @CurrentStandardParent
						break
					end 
			end
			else
			begin
				select @Parrent = @IsHiden
				break
			end
		end

		select top 1 @Name = Name, @Description = [Description], @StandardSubjectRef = StandardSubjectRef, @LowerGradeLevelRef = LowerGradeLevelRef, 
			@UpperGradeLevelRef = UpperGradeLevelRef, @IsActive = IsActive, @AcademicBenchmarkId = AcademicBenchmarkId from @CurrentClassStandards CC where CC.Id = @StandartId

		insert into @StandardTree (Id, ParentStandardRef, Name, [Description], StandardSubjectRef, LowerGradeLevelRef, UpperGradeLevelRef, IsActive, AcademicBenchmarkId, [Column], Done, IsSelected) Values
			(@StandartId, @Parrent, @Name, @Description, @StandardSubjectRef, @LowerGradeLevelRef, @UpperGradeLevelRef, @IsActive, @AcademicBenchmarkId, -1, 0, 0)

		select @Parrent = null
		
		fetch next from ResultTableCursor into @StandartId, @IsHiden;
	end

	close ResultTableCursor;
	deallocate ResultTableCursor;

	select top 1 @StandartId = Id, @Parrent = ParentStandardRef, @Done = Done from @StandardTree where Done = 0 order by ParentStandardRef ASC

	while @StandartId is not null
	begin
		set @Column = null

		if @Parrent is null
			set @Column = 0
		else
			select @Column = ST.[Column] + 1 from @StandardTree ST where ST.Id = @Parrent and ST.Done = 1
		
		update @StandardTree set Done = 1, [Column] = @Column where Id = @StandartId

		select @StandartId = null

		select top 1 @StandartId = Id, @Parrent = ParentStandardRef, @Done = Done from @StandardTree where Done = 0 order by ParentStandardRef ASC
	end

	select top 1 @StandartId = Id, @Parrent = ParentStandardRef from @StandardTree where Id = @StandardIdForSearch

	update @StandardTree set IsSelected = 1 where Id = @StandartId;
	
	while @Parrent is not null
	begin
		update @StandardTree set IsSelected = 1 where Id = @Parrent;
		select top 1 @Parrent = ParentStandardRef from @StandardTree where Id = @Parrent
	end
	
	select ST.Id, ST.ParentStandardRef, ST.Name, ST.[Description], ST.StandardSubjectRef, ST.LowerGradeLevelRef, ST.UpperGradeLevelRef, ST.IsActive, ST.AcademicBenchmarkId, ST.[Column], ST.IsSelected, cast(row_number() over (partition by [Column] order by [Column]) as int) as Row
	from @StandardTree ST
	order by [Column] asc

GO
