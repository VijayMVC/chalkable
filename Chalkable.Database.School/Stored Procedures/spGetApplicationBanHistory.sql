


Create Procedure spGetApplicationBanHistory @applicationId uniqueidentifier
as
Select
ApplicationBanHistory.*,
Person.FirstName as PersonFisrtName,
Person.LastName as PersonLastName
From
ApplicationBanHistory
Join
Person on Person.Id = ApplicationBanHistory.PersonRef
Where ApplicationRef = @applicationId
Order By ApplicationBanHistory.[Date] Desc