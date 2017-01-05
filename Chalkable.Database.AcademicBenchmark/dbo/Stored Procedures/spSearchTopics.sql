Create Procedure [dbo].[spSearchTopics]
	@searchQuery nvarchar(500),
	@deepest bit,
	@start int,
	@count int
As

Select count(Id) As AllCount
From [Topic]
Where 
	(@searchQuery is null 
	 Or [Description] like(@searchQuery))
  AND
	(@deepest is null or IsDeepest = @deepest)

Select * From [Topic]
Where 
	(@searchQuery is null 
	 Or [Description] like(@searchQuery))
  AND
	(@deepest is null or IsDeepest = @deepest)
Order By Id
OFFSET  @start ROWS FETCH NEXT @count ROWS ONLY