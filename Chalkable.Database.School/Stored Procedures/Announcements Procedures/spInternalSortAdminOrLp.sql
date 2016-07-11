CREATE Procedure [dbo].[spInternalSortAdminOrLp] 
	@lessonPlans TLessonPlan READONLY, 
	@adminAnn TAdminAnnouncement READONLY, 
	@annType bit, 
	@filterOption int, 
	@sortOption int, 
	@sortType bit, 
	@pFrom sql_variant, 
	@pTo sql_variant, 
	@start int, 
	@count int,
	@includeFrom bit,
	@includeTo bit

AS

declare @t Table
	(
		Id int,
		FilteredField sql_variant,
		SortedField sql_variant
	)

declare 
	@SORT_BY_DATE int = 0,
	@SORT_BY_TITLE int = 1,
	@SORT_BY_CLASS_NAME int = 2,
	@LESSON_PLAN_TYPE int  = 0,
	@ADMIN_ANN_TYPE int = 1,
	@ASC_SORT bit = 0,
	@DESC_SORT bit = 1,
	@NOTINCLUDE bit = 0

-- insert AdminAnnouncements or LessonPlans to temp table
If(@annType = @LESSON_PLAN_TYPE)
	if(@filterOption = @SORT_BY_DATE and @sortOption = @SORT_BY_DATE)
		Insert @t select Id, StartDate, StartDate from @lessonPlans
	else
		if(@filterOption = @SORT_BY_DATE and @sortOption <> @SORT_BY_DATE)
			Insert @t 
				select Id, 
				StartDate, 
				case @sortOption
					when @SORT_BY_TITLE then Title
					when @SORT_BY_CLASS_NAME then ClassName
				end 
			from @lessonPlans
		else
			if(@filterOption <> @SORT_BY_DATE and @sortOption = @SORT_BY_DATE)
				Insert @t 
					select Id, 
					case @filterOption
						when @SORT_BY_TITLE then Title
						when @SORT_BY_CLASS_NAME then ClassName
					end,
					StartDate
				from @lessonPlans
			else
				Insert @t 
					select Id, 
					case @filterOption
						when @SORT_BY_TITLE then Title
						when @SORT_BY_CLASS_NAME then ClassName
					end,
					case @sortOption
						when @SORT_BY_TITLE then Title
						when @SORT_BY_CLASS_NAME then ClassName
					end
				from @lessonPlans
else
	if(@filterOption = @SORT_BY_DATE and @sortOption = @SORT_BY_DATE)
		Insert @t select Id, Expires, Expires from @adminAnn
	else
		if(@filterOption = @SORT_BY_DATE and @sortOption <> @SORT_BY_DATE)
			Insert @t select Id, Expires, Title from @adminAnn
		else
			if(@filterOption <> @SORT_BY_DATE and @sortOption = @SORT_BY_DATE)
				Insert @t select Id, Title, Expires from @adminAnn
			else
				Insert @t select Id, Title, Title from @adminAnn

--	cut by sortOprion with includes or no
	if(@pFrom is not null)
		if(@includeFrom = @NOTINCLUDE)
			delete from @t where FilteredField <= @pFrom
		else 
			delete from @t where FilteredField < @pFrom

	if(@pTo is not null) 
		if(@includeTo = @NOTINCLUDE)
			delete from @t where FilteredField >= @pTo
		else 
			delete from @t where FilteredField > @pTo

--order and result select
If(@annType = @LESSON_PLAN_TYPE)
	select LP.* From @t T
	Join @lessonPlans LP on LP.Id = T.id
	order by
		(case 
			when @sortType = @ASC_SORT then T.SortedField
		end) ASC,
		(case 
			when @sortType = @DESC_SORT then T.SortedField
		end) DESC
		OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
else
	select AA.* From @t T
	Join @adminAnn AA on AA.Id = T.id
	order by
		(case 
			when @sortType = @ASC_SORT then T.SortedField
		end) ASC,
		(case 
			when @sortType = @DESC_SORT then T.SortedField
		end) DESC
		OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

GO


