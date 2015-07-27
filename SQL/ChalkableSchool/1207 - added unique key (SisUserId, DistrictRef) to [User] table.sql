Create Unique NonClustered Index UQ_User_DistrictRef_SisUserId ON [dbo].[user]
(
	SisUserId, DistrictRef
)
Where (SisUserId IS NOT NULL)
Go
