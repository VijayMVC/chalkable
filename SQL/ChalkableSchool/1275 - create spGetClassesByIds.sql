Create Procedure spGetClassesByIds
	@ids TInt32 ReadOnly
As

select * from Class
where Id in(select * from @ids)

Go