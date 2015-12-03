Delete From [UserLoginInfo]
Where Id in (
				Select data.Id From 
				(
					Select Id, SisUserId, DistrictRef, row_number() over (partition by SisUserId, DistrictRef order by SisUserId, DistrictRef) as RowNumber 
					From [User]
					Where SisUserId is not null
				) data
				Where data.RowNumber > 1
			)


Delete From [User]
Where Id in (
				Select data.Id From 
				(
					Select Id, SisUserId, DistrictRef, row_number() over (partition by SisUserId, DistrictRef order by SisUserId, DistrictRef) as RowNumber 
					From [User]
					Where SisUserId is not null
				) data
				Where data.RowNumber > 1
			)

Go


Create Unique NonClustered Index UQ_User_DistrictRef_SisUserId ON [dbo].[user]
(
	SisUserId, DistrictRef
)
Where (SisUserId IS NOT NULL)
Go
