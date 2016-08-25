﻿CREATE Procedure spSearchStandards
	@searchQuery nvarchar(500),
	@deepest bit,
	@start int,
	@count int
As

Select count(Id) As AllCount
From [Standard]
Where 
	(@searchQuery is null 
	 Or [Description] like(@searchQuery)
	 Or [Number]      like(@searchQuery)
	 Or [Label]	      like(@searchQuery)
	 Or [ExtDescription] like(@searchQuery))
  AND
	(@deepest is null or IsDeepest = @deepest)

Select * From [Standard]
Where 
	(@searchQuery is null 
	 Or [Description] like(@searchQuery)
	 Or [Number]      like(@searchQuery)
	 Or [Label]	      like(@searchQuery)
	 Or [ExtDescription] like(@searchQuery))
  AND
	(@deepest is null or IsDeepest = @deepest)
Order By Number
OFFSET  @start ROWS FETCH NEXT @count ROWS ONLY

Go