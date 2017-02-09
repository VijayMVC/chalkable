CREATE TABLE [dbo].[User] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [Login]                 NVARCHAR (256)   NOT NULL,
    [Password]              NVARCHAR (256)   NOT NULL,
    [IsSysAdmin]            BIT              NOT NULL,
    [IsDeveloper]           BIT              NOT NULL,
    [ConfirmationKey]       NVARCHAR (256)   NULL,
    [SisUserName]           NVARCHAR (256)   NULL,
    [DistrictRef]           UNIQUEIDENTIFIER NULL,
    [SisUserId]             INT              NULL,
    [FullName]              NVARCHAR (1024)  NULL,
    [IsAppTester]           BIT              NOT NULL,
    [IsDistrictRegistrator] BIT              NOT NULL,
	[IsAssessmentAdmin]		BIT              NOT NULL DEFAULT (0),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id]),
    CONSTRAINT [UQ_Login] UNIQUE NONCLUSTERED ([Login] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_USER_LOGIN_PASSWORD]
    ON [dbo].[User]([Login] ASC, [Password] ASC);


GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_User_DistrictRef_SisUserId]
    ON [dbo].[User]([SisUserId] ASC, [DistrictRef] ASC) WHERE ([SisUserId] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [IX_District_SisUser]
    ON [dbo].[User]([SisUserId] ASC, [DistrictRef] ASC);


GO

Create Trigger UserDeleteTrigger
On [User]
After Delete
as
	If Exists(
			Select * From SchoolUser 
			join deleted on deleted.SisUserId = SchoolUser.UserRef and deleted.DistrictRef = SchoolUser.DistrictRef
		)
		Throw 51001, 'User can not be deleted. SchoolUser has record with such SisUserId and DistrictRef', 1;
GO

CREATE Trigger [dbo].[UserLogTrigger]
On [dbo].[User]
After Insert, Update, Delete
as 

	Insert into UserLog
	(Id, SisUserId, DistrictRef, Added, Operation)
	select 
		newid(), SisUserId, DistrictRef, getdate(), 1
	from Inserted
	Where SisUserId is not null

	Insert into UserLog
	(Id, SisUserId, DistrictRef, Added, Operation)
	select 
		newid(), SisUserId, DistrictRef, getdate(), 2
	from Deleted
		Where SisUserId is not null
GO