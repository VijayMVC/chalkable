Drop procedure [spUpdateAdminAnnouncementData]
Go

Create Procedure spUpdateAnnouncementRecipientData @announcementId Int, @personId Int, @complete Bit
As
Begin Transaction 
Declare @currentComplete Bit = (Select Top 1 Complete From AnnouncementRecipientData 
								Where AnnouncementRef = @announcementId and PersonRef = @personId)

If @currentComplete is null
Begin
	Insert Into AnnouncementRecipientData(AnnouncementRef, PersonRef, Complete)
	Values (@announcementId, @personId, @complete) 
End
Else If @currentComplete <> @complete
Begin
	Update AnnouncementRecipientData
	Set Complete = @complete
	Where AnnouncementRef = @announcementId and PersonRef = @personId
End
Commit Transaction
GO



