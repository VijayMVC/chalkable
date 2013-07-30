create procedure spGetAnnouncementRecipientPersons @announcementId uniqueidentifier, @callerId uniqueidentifier 
as
declare @classId uniqueidentifier = (select top 1 MarkingPeriodClass.ClassRef from Announcement
									 join MarkingPeriodClass on MarkingPeriodClass.Id = Announcement.MarkingPeriodClassRef
									 where Announcement.Id = @announcementId)

if(@classId is not null)
begin
	select * from vwPerson vwPerson
	where exists(select * from ClassPerson where ClassRef = @classId and PersonRef = vwPerson.Id)
	and vwPerson.Id <> @callerId
end
else
begin
	declare @annRecipientR table
	(
		Id uniqueidentifier,
		AnnouncementRef uniqueidentifier,
		ToAll bit,
		RoleRef int, 
		GradeLevelRef uniqueidentifier,
		PersonRef uniqueidentifier
	)

	insert into @annRecipientR
	select * from AnnouncementRecipient
	where AnnouncementRef = @announcementId

	if(exists(select * from @annRecipientR where ToAll = 1))
		select * from vwPerson
		where Id <> @callerId
	else
	begin
		select p.* from vwPerson p 
		where exists
					(
						select * from @annRecipientR ar
						where ar.RoleRef = p.RoleRef or ar.GradeLevelRef = p.GradeLevel_Id
							  or ar.PersonRef = p.Id
					)
				and p.Id <> @callerId
	end
end