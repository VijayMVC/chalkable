Insert into UserLoginInfo
(Id)
Select 
	Id 
from 
	[user] 
where 
	IsSysAdmin = 1 or IsDeveloper = 1