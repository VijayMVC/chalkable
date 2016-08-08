CREATE PROCEDURE [dbo].[spInternalSortAdminOrLp] 
	@lessonPlans TLessonPlan READONLY, 
	@adminAnn TAdminAnnouncement READONLY, 
	@supplementalAnn TSupplementalAnnouncement READONLY,
	@annType int, 
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

declare @t TAnnouncementOrder

declare 
	@SORT_BY_DATE int = 0,
	@SORT_BY_TITLE int = 1,
	@SORT_BY_CLASS_NAME int = 2,
	@LESSON_PLAN_TYPE int  = 0,
	@ADMIN_ANN_TYPE int = 1,
	@SUPPLEMENTAL_ANN_TYPE int = 2,
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

If(@annType = @ADMIN_ANN_TYPE)
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

If(@annType = @SUPPLEMENTAL_ANN_TYPE)
	if(@filterOption = @SORT_BY_DATE and @sortOption = @SORT_BY_DATE)
		Insert @t select Id, Expires, Expires from @supplementalAnn
	else
		if(@filterOption = @SORT_BY_DATE and @sortOption <> @SORT_BY_DATE)
			Insert @t 
				select Id, 
					Expires, 
					case @sortOption
						when @SORT_BY_TITLE then Title
						when @SORT_BY_CLASS_NAME then ClassName
					end 
				from @supplementalAnn
		else
			if(@filterOption <> @SORT_BY_DATE and @sortOption = @SORT_BY_DATE)
				Insert @t 
					select Id, 
						   case @filterOption
								when @SORT_BY_TITLE then Title
								when @SORT_BY_CLASS_NAME then ClassName
						   end,
						   Expires
				from @supplementalAnn
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
					from @supplementalAnn


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

--result select
If(@annType = @LESSON_PLAN_TYPE)
Begin
	exec spSelectLessonPlans @lessonPlans, @t, @sortType, @start, @count
End

if(@annType = @ADMIN_ANN_TYPE)
Begin
	exec spSelectAdminAnnoucnement @adminAnn, @t, @sortType, @start, @count
End

if(@annType = @SUPPLEMENTAL_ANN_TYPE)
	declare @sortedSA TSupplementalAnnouncement
	
	Insert Into @sortedSA
	select SA.* From @t T
	Join @supplementalAnn SA on SA.Id = T.Id
	order by (case when @sortType = @ASC_SORT then T.SortedField end) ASC,
				(case when @sortType = @DESC_SORT then T.SortedField end) DESC
		OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

	exec spSelectSupplementalAnnouncements @sortedSA
GO