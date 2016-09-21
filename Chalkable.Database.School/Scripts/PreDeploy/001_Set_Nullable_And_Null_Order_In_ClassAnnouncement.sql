If (Object_Id('[dbo].[ClassAnnouncement]') Is Not Null And EXISTS(SELECT * FROM sys.columns WHERE Name = 'Order' AND Object_ID = Object_ID('[dbo].[ClassAnnouncement]')))
Begin
	ALTER TABLE [dbo].[ClassAnnouncement] ALTER COLUMN [Order] INT NULL
	UPDATE [dbo].[ClassAnnouncement] SET [Order] = NULL 
End