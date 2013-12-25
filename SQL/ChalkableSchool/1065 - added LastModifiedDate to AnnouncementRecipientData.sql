alter table AnnouncementRecipientData
add LastModifiedDate datetime2 null 
go

ALTER procedure [dbo].[spUpdateAnnouncemetRecipientData] @personId uniqueidentifier, @announcementId uniqueidentifier,
														  @starred bit, @starredAutomatically int, @currentDate date
as

declare @id uniqueidentifier, @oldStarredAutomatically bit
select @id = anr.id, @oldStarredAutomatically = anr.StarredAutomatically
from AnnouncementRecipientData anr
where anr.AnnouncementRef = @announcementId 
and anr.PersonRef = @personId

if @id is not null
begin 
	 if(@starredAutomatically is null)
		set @starredAutomatically = @oldStarredAutomatically
	 update AnnouncementRecipientData 
	 set Starred = @starred , StarredAutomatically = @starredAutomatically, LastModifiedDate = @currentDate
	 where @id = AnnouncementRecipientData.Id
end
else 
begin
     insert into AnnouncementRecipientData(AnnouncementRef, PersonRef, Starred, StarredAutomatically, LastModifiedDate)
     values (@announcementId, @personId, @starred, @starredAutomatically, @currentDate)
end

GO


ALTER procedure [dbo].[spApplyStarringAnnouncementForStudent] @personId uniqueidentifier, @currentDate date
as

DECLARE @id uniqueidentifier, @classId uniqueidentifier,
		 @ExpDay date, @AnnTypeId int, @saterredAnnRecipientDate int
DECLARE @currentDay date = @currentDate
DECLARE @tomorrowDate date = dateAdd(day, 1, @currentDay)
DECLARE @dueDays1 int = 0
DECLARE @dueDays2 int = 0
DECLARE @starredA int = 0
DECLARE AnnouncementStCursor CURSOR FOR
select an.Id,
       mkc.ClassRef,
       an.Expires,
       an.AnnouncementTypeRef,
       ard.StarredAutomatically
from
 Announcement an
left join MarkingPeriodClass mkc on mkc.Id = an.MarkingPeriodClassRef
left join ClassPerson c on c.ClassRef = mkc.ClassRef and c.PersonRef = @personId
left join AnnouncementRecipientData ard on ard.AnnouncementRef = an.Id and ard.PersonRef = @personId

where 
	  ((an.MarkingPeriodClassRef is not null and c.PersonRef = @personId)
	   or(an.AnnouncementTypeRef = 11 and exists(select * from AnnouncementRecipient ar 
								 where ar.AnnouncementRef = an.Id and (ar.ToAll = 1 or ar.PersonRef = @personId or ar.RoleRef = 3))
		 )
	  )
	  and ard.LastModifiedDate < @currentDate 
	  and an.[State] <> 0 and(ard.Id is null or (ard.Starred <> 0 and an.Expires < @currentDate) 
											or (ard.Starred <> 1 and an.Expires = @tomorrowDate))
OPEN AnnouncementStCursor
FETCH NEXT FROM AnnouncementStCursor
INTO @id, @classId, @ExpDay, @AnnTypeId, @saterredAnnRecipientDate


declare @studentAvgT table(classId uniqueidentifier, [avg] int)

WHILE @@FETCH_STATUS = 0
BEGIN 
     if @ExpDay < @currentDay 
     BEGIN
          exec spUpdateAnnouncemetRecipientData
               @personId,
               @id, 0, @starredA, @currentDate
     END
     else
     BEGIN
	 
	      set @starredA = 0
          if(@saterredAnnRecipientDate is not null)
          begin
                set @starredA = @saterredAnnRecipientDate
          end  
		 
          if @ExpDay = DATEADD(Day, 1, @currentDay) and @starredA < 3
          BEGIN
			   set @starredA = 3
               exec spUpdateAnnouncemetRecipientData
               @personId,
               @id, 1, @starredA, @currentDate
          END
          else
          BEGIN    
               exec dbo.spGetDueDays @AnnTypeId, @dueDays1 output, @dueDays2 output
        	   DECLARE @avgStudent int = (select top 1 [avg] from @studentAvgT where classId = @classId)
			   if(@avgStudent is null)
			   begin
					--TODO : think about this ... include type percents
				    set @avgStudent = (
	 									select Avg(sa.GradeValue) 
										from ClassPerson csp
										join StudentAnnouncement sa on sa.ClassPersonRef = csp.Id
										where csp.ClassRef = @classId and csp.PersonRef = @personId
								      )
					if(@avgStudent is null)
						set @avgStudent = 0
                    insert into @studentAvgT(classId, [avg])
					values(@classId, @avgStudent)
			   end
			   if (@dueDays1 != 0) 
                  and
                   ((((@ExpDay <= DATEADD(Day, @dueDays1, @currentDay) and @starredA < 2)  or (@dueDays2!=0 and @ExpDay <= DATEADD(Day, @dueDays2, @currentDay) and @starredA < 1)) 
                       and  @avgStudent >= 80)
                    or
                     (((@ExpDay <= DATEADD(Day, @dueDays1 + 1, @currentDay) and @starredA < 2) or (@dueDays2!=0 and @ExpDay <= DATEADD(Day, @dueDays2 + 1, @currentDay)and @starredA < 1)) 
                       and @avgStudent between 65 and 80)
                    or
                     (((@ExpDay <= DATEADD(Day, @dueDays1 + 2, @currentDay) and @starredA < 2) or (@dueDays2!=0 and @ExpDay <= DATEADD(Day, @dueDays2 + 2, @currentDay)and @starredA < 1))
                       and @avgStudent <= 65))
                 
               BEGIN
                    set @starredA = @starredA + 1
                    exec spUpdateAnnouncemetRecipientData
                    @personId,
                    @id, 1, @starredA, @currentDate
               END
               set @dueDays1 = 0
               set @dueDays2 = 0
          END
     END
     FETCH NEXT FROM AnnouncementStCursor
     INTO @id, @classId, @ExpDay, @AnnTypeId, @saterredAnnRecipientDate
END
CLOSE AnnouncementStCursor;
DEALLOCATE AnnouncementStCursor;

GO


ALTER procedure [dbo].[spApplyStarringAnnouncementForTeacher] @personId uniqueidentifier, @currentDate date
as
DECLARE @id uniqueidentifier, @ExpDay date, @starredA int
DECLARE @currentDay date = @currentDate
DECLARE @nextDay date = DATEADD(Day, 1, @currentDay)
DECLARE AnnouncementCursorToUnstar CURSOR FOR
select x.AnnouncmentId, x.AnExpires, ard.StarredAutomatically 
from AnnouncementRecipientData ard
right join 
(select an.Id as AnnouncmentId, an.Expires as AnExpires , @personId as AnSchoolPerson
 from Announcement an
 left join AnnouncementRecipient ar on ar.AnnouncementRef = an.Id
 where  an.PersonRef = @personId or (ar.Id is not null and
			  (ar.PersonRef = @personId  or ar.ToAll = 1 or ar.RoleRef = 2)))x
on x.AnnouncmentId = ard.AnnouncementRef and x.AnSchoolPerson = ard.PersonRef
where  x.AnExpires < @currentDay
	and ard.StarredAutomatically = 1
	and (ard.Id is null or ard.Starred <> 0)
	and ard.LastModifiedDate < @currentDay

OPEN AnnouncementCursorToUnstar
FETCH NEXT FROM AnnouncementCursorToUnstar
INTO @id, @ExpDay, @starredA
WHILE @@FETCH_STATUS = 0
BEGIN 
     exec spUpdateAnnouncemetRecipientData @personId, @id, 0, 0, @currentDay     
     
     FETCH NEXT FROM AnnouncementCursorToUnstar
     INTO @id, @ExpDay, @starredA
END
CLOSE AnnouncementCursorToUnstar;
DEALLOCATE AnnouncementCursorToUnstar;


DECLARE AnnouncementCursorToStar CURSOR FOR
select x.AnnouncmentId, x.AnExpires, ard.StarredAutomatically 
from AnnouncementRecipientData ard
right join 
(select an.Id as AnnouncmentId, an.Expires as AnExpires , @personId as AnSchoolPerson
 from Announcement an
 left join AnnouncementRecipient ar on ar.AnnouncementRef = an.Id
 where an.PersonRef = @personId or (ar.Id is not null and
			  (ar.PersonRef = @personId  or ar.ToAll = 1 or ar.RoleRef = 2)))x
on x.AnnouncmentId = ard.AnnouncementRef and x.AnSchoolPerson = ard.PersonRef
where x.AnExpires = @nextDay
	and (ard.StarredAutomatically < 1 or ard.StarredAutomatically is null)
	and (ard.Id is null or ard.Starred <> 1)
	and ard.LastModifiedDate < @currentDay

OPEN AnnouncementCursorToStar
FETCH NEXT FROM AnnouncementCursorToStar
INTO @id, @ExpDay, @starredA
WHILE @@FETCH_STATUS = 0
BEGIN 
     exec spUpdateAnnouncemetRecipientData @personId, @id, 1, 1, @currentDay
     
     FETCH NEXT FROM AnnouncementCursorToStar
     INTO @id, @ExpDay, @starredA
END
CLOSE AnnouncementCursorToStar;
DEALLOCATE AnnouncementCursorToStar;

GO





