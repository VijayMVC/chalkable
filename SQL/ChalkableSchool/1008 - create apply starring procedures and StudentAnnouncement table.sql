create procedure [dbo].[spGetDueDays] @AnnTypeId int, @dueDays1 int output, @dueDays2 int output
as

--Id	Name
--1		Announcement
--2		HW
--3		Essay
--4		Quiz
--5		Test
--6		Project
--7		Final
--8		Midterm
--9		Book report
--10	Term paper
--11	Admin


  if @AnnTypeId != 11 and @AnnTypeId != 1 
  begin
	   
       
	   if @AnnTypeId = 2
		  set @dueDays1 = 2
	   else 
		  set @dueDays1 = 3

       if @AnnTypeId = 6 or @AnnTypeId = 10 
          set @dueDays2 = 14
       else
       begin
            if @AnnTypeId != 5 and @AnnTypeId != 2 and @AnnTypeId != 4
               set @dueDays2 = 7
       end
  end
GO
alter table Announcement
drop constraint FK_Announcement_AnnouncementType
go
alter table Announcement
drop column AnnouncementTypeRef 
go
drop table AnnouncementType
go
CREATE TABLE [dbo].[AnnouncementType](
	[Id] int primary key identity NOT NULL,
	[IsSystem] [bit] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	Gradable [bit] NULL,
)
GO
alter table Announcement
add AnnouncementTypeRef int not null constraint FK_Announcement_AnnouncementType foreign key references AnnouncementType(Id)
go
set identity_insert AnnouncementType on

insert into AnnouncementType
(Id, IsSystem, [Name], [Description], [Gradable])
values
(1, 1, 'Announcement', 'Announcement', 0),
(2, 1, 'HW', 'Homework', 1),
(3, 1, 'Essay', 'Essay', 1),
(4, 1, 'Quiz', 'Quiz', 1),
(5, 1, 'Test', 'Test', 1),
(6, 1, 'Project', 'Project', 1),
(7, 1, 'Final', 'Final', 1),
(8, 1, 'Midterm', 'Midterm', 1),
(9, 1, 'Book report', 'Book report', 1),
(10, 1, 'Term paper', 'Term paper', 1),
(11, 1, 'Admin', 'Admin', 0)

set identity_insert AnnouncementType off
go


create procedure [dbo].[spUpdateAnnouncemetRecipientData] @personId uniqueidentifier, @announcementId uniqueidentifier,
														  @starred bit, @starredAutomatically int
as
declare @id uniqueidentifier = (select anr.id
                   from AnnouncementRecipientData anr
                   where anr.AnnouncementRef = @announcementId 
                   and anr.PersonRef = @personId);

if @id is not null
begin 

     update AnnouncementRecipientData 
     set Starred = @starred , StarredAutomatically = @starredAutomatically
     where @id = AnnouncementRecipientData.Id
end
else 
begin
     insert into AnnouncementRecipientData(AnnouncementRef, PersonRef, Starred, StarredAutomatically)
     values (@announcementId, @personId, @starred, @starredAutomatically)
end
GO

create table StudentAnnouncement
(
	Id uniqueidentifier not null  primary key,
	ClassPersonRef uniqueidentifier not null constraint FK_StudentAnnouncement_ClassPerson foreign key references ClassPerson(Id),
	AnnouncementRef uniqueidentifier not null constraint FK_StudentAnnouncement_Announcement foreign key references Announcement(Id),
	Comment nvarchar(1024) null,
	GradeValue int null,
	ExtraCredit nvarchar(255) null,
	Dropped bit not null,
	State int not null,
	ApplicationRef uniqueidentifier null
)
go



create procedure [dbo].[spApplyStarringAnnouncementForStudent] @personId uniqueidentifier, @currentDate date
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

where ((an.MarkingPeriodClassRef is not null and c.PersonRef = @personId)
	   or(an.AnnouncementTypeRef = 11 and exists(select * from AnnouncementRecipient ar 
								 where ar.AnnouncementRef = an.Id and (ar.ToAll = 1 or ar.PersonRef = @personId or ar.RoleRef = 3))
		 )
	  ) 
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
               @id, 0, @starredA
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
               @id, 1, @starredA
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
                    @id, 1, @starredA
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


create procedure [dbo].[spApplyStarringAnnouncementForTeacher] @personId uniqueidentifier, @currentDate date
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

OPEN AnnouncementCursorToUnstar
FETCH NEXT FROM AnnouncementCursorToUnstar
INTO @id, @ExpDay, @starredA
WHILE @@FETCH_STATUS = 0
BEGIN 
     exec spUpdateAnnouncemetRecipientData @personId, @id, 0, 0     
     
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

OPEN AnnouncementCursorToStar
FETCH NEXT FROM AnnouncementCursorToStar
INTO @id, @ExpDay, @starredA
WHILE @@FETCH_STATUS = 0
BEGIN 
     exec spUpdateAnnouncemetRecipientData @personId, @id, 1, 1
     
     FETCH NEXT FROM AnnouncementCursorToStar
     INTO @id, @ExpDay, @starredA
END
CLOSE AnnouncementCursorToStar;
DEALLOCATE AnnouncementCursorToStar;
GO



