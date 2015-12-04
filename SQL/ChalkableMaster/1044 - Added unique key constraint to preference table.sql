Delete From Preference Where Id not in
(
	Select  
		max(Id) 
	From 
		Preference
	Group By 
		[Key]
)

GO

Alter Table Preference
	Add Constraint UQ_KEY Unique ([Key])

GO