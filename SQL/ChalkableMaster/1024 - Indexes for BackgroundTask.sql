
CREATE NONCLUSTERED INDEX 
	[IX_BackgroundTask_District] ON BackgroundTask(DistrictRef)
GO

CREATE NONCLUSTERED INDEX 
	[IX_BackgroundTask_State] ON BackgroundTask(State)
GO

CREATE NONCLUSTERED INDEX 
	[IX_BackgroundTask_Scheduled] ON BackgroundTask(Scheduled)
GO