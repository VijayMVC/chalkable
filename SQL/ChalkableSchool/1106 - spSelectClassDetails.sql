Create Procedure spSelectClassDetails
	@classes TClassDetails readonly
as
	

select * from 
	@classes
	order by Class_Id


select mpc.* 
from MarkingPeriodClass mpc
	join @classes c on c.Class_Id = mpc.ClassRef
