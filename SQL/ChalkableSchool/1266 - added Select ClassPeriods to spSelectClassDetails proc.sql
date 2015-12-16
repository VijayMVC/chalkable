Alter Procedure [dbo].[spSelectClassDetails]
	@classes TClassDetails readonly
as
	Select 
		* 
	From
		@classes
	Order By 
		Class_Id

	Select 
		mpc.*
	From 
		MarkingPeriodClass mpc
		join @classes c on c.Class_Id = mpc.ClassRef

	Select 
		ct.*
	From 
		ClassTeacher ct
		join @classes c on c.Class_Id = ct.ClassRef

	Select 
		ClassPeriod.*, 
		Period.*
	From 
		ClassPeriod
		join @classes c on c.Class_Id = ClassPeriod.ClassRef
		join Period on Period.Id = ClassPeriod.PeriodRef 
	
GO