alter VIEW [dbo].[vwPrivateMessage]
AS
	SELECT
		 PrivateMessage.Id as PrivateMessage_Id,
		 PrivateMessage.Body as PrivateMessage_Body,
		 PrivateMessage.[Read] as PrivateMessage_Read,
		 PrivateMessage.[Sent] as PrivateMessage_Sent,
		 PrivateMessage.[Subject] as PrivateMessage_Subject,
		 PrivateMessage.DeletedBySender as PrivateMessage_DeletedBySender,
		 PrivateMessage.DeletedByRecipient as PrivateMessage_DeletedByRecipient,
		 PrivateMessage.ToPersonRef as PrivateMessage_ToPersonRef,
		 PrivateMessage.FromPersonRef as PrivateMessage_FromPersonRef,
		 p.FirstName as PrivateMessage_SenderFirstName,
		 p.LastName as PrivateMessage_SenderLastName,
		 p.Gender as PrivateMessage_SenderGender,
		 p.Salutation as PrivateMessage_SenderSalutation,
		 p.RoleRef as PrivateMessage_SenderRoleRef,
		 p.SchoolRef as PrivateMessage_SenderSchoolRef,
		 
		 p.HasMedicalAlert as PrivateMessage_SenderHasMedicalAlert,
		 p.IsAllowedInetAccess as PrivateMessage_SenderIsAllowedInetAccess,
		 p.SpecialInstructions as PrivateMessage_SenderSpecialInstructions,
		 p.SpEdStatus as PrivateMessage_SenderSpEdStatus,
		 
		 p2.FirstName as PrivateMessage_RecipientFirstName,
		 p2.LastName as PrivateMessage_RecipientLastName,
		 p2.Gender as PrivateMessage_RecipientGender,
		 p2.Salutation as PrivateMessage_RecipientSalutation,
		 p2.RoleRef as PrivateMessage_RecipientRoleRef,
		 p2.SchoolRef as PrivateMessage_RecipientSchoolRef,
		 
		 p2.HasMedicalAlert as PrivateMessage_RecipientHasMedicalAlert,
		 p2.IsAllowedInetAccess as PrivateMessage_RecipientIsAllowedInetAccess,
		 p2.SpecialInstructions as PrivateMessage_RecipientSpecialInstructions,
		 p2.SpEdStatus as PrivateMessage_RecipientSpEdStatus
	FROM 
		PrivateMessage 
		join vwPerson p on p.Id = PrivateMessage.FromPersonRef
		join vwPerson p2 on p2.Id = PrivateMessage.ToPersonRef 
GO


