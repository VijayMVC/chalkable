 Create type TIntId as Table
(
	Id int
)
Go

 
 Create View vwClassAnnouncement
 As
 Select
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.[State] as [State],
	Announcement.Content as Content,
	Announcement.Title as [Title],
	ClassAnnouncement.Expires as Expires,
	ClassAnnouncement.[Order] as [Order],
	ClassAnnouncement.Dropped as Dropped,
	ClassAnnouncement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	ClassAnnouncement.SchoolRef as SchoolRef,
	ClassAnnouncement.SisActivityId as SisActivityId,
    ClassAnnouncement.MaxScore as MaxScore,
    ClassAnnouncement.WeightAddition as WeightAddition,
    ClassAnnouncement.WeightMultiplier as WeightMultiplier,
    ClassAnnouncement.MayBeDropped as MayBeDropped,
	ClassAnnouncement.VisibleForStudent as VisibleForStudent,
	ClassAnnouncement.ClassRef as ClassRef,

	Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
	Staff.Gender as PrimaryTeacherGender,
	Class.Name as ClassName,
	Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
	Class.MinGradeLevelRef as MinGradeLevelId,
	Class.MaxGradeLevelRef as MaxGradeLevelId,
	Class.PrimaryTeacherRef as PrimaryTeacherRef,  
	Class.ChalkableDepartmentRef as DepartmentId

 From ClassAnnouncement
 Join Announcement on Announcement.Id = ClassAnnouncement.Id
 Join Class on Class.Id = ClassAnnouncement.ClassRef
 Left Join Staff on Staff.Id = Class.PrimaryTeacherRef

 Go


Create Type TClassAnnouncement as table
(
	Id int not null,
	Created dateTime2 not null,
	[State] int not null,
	Content nvarchar(max),
	[Title] nvarchar(max),
	
	Expires dateTime2 not null,
	[Order] int not null,
	Dropped bit not null,
	ClassAnnouncementTypeRef int null,
	SchoolRef int null,
	SisActivityId int null,
    MaxScore decimal null,
    WeightAddition decimal(9,6) null,
    WeightMultiplier decimal(9,6) null,
    MayBeDropped bit,
	VisibleForStudent bit,
	ClassRef int not null,

	PrimaryTeacherName nvarchar(max),
	PrimaryTeacherGender nvarchar(max),
	ClassName nvarchar(max),
	FullClassName nvarchar(max),
	MinGradeLevelId int,
	MaxGradeLevelId int,
	PrimaryTeacherRef int,
	DepartmentId uniqueidentifier,
	IsOwner bit,
	Complete bit,
	AllCount int
)
GO 

 Create View vwAdminAnnouncement
 As
 Select
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.[State] as [State],
	Announcement.Content as Content,
	Announcement.Title as [Title],
	AdminAnnouncement.Expires as Expires,
	AdminAnnouncement.AdminRef as AdminRef,
	Person.FirstName + ' ' + Person.LastName as AdminName,
	Person.Gender as AdminGender

 From AdminAnnouncement
 Join Announcement on Announcement.Id = AdminAnnouncement.Id
 Join Person on Person.Id = AdminAnnouncement.AdminRef
 Go


 Create Type TAdminAnnouncement As Table
 (
	Id int not null,
	Created dateTime2 not null,
	[State] int not null,
	Content nvarchar(max),
	[Title] nvarchar(max),
	Expires dateTime2 not null,
	AdminRef int not null,
	AdminName nvarchar(max),
	AdminGendeer nvarchar(max),
	IsOwner bit,
	Complete bit,
	AllCount int
 )
 Go

 Create View vwLessonPlan
 As
 Select
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.[State] as [State],
	Announcement.Content as Content,
	Announcement.Title as [Title],
	LessonPlan.ClassRef as ClassRef,
	LessonPlan.SchoolRef as SchoolRef,
	LessonPlan.StartDate as StartDate,
	LessonPlan.EndDate as EndDate,
	LessonPlan.GalleryCategoryRef as GalleryCategoryRef,
	LessonPlan.VisibleForStudent as VisibleForStudent,
	
	Staff.FirstName + ' ' + Staff.LastName as PrimaryTeacherName,
	Staff.Gender as PrimaryTeacherGender,
	Class.Name as ClassName,
	Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as FullClassName,
	Class.MinGradeLevelRef as MinGradeLevelId,
	Class.MaxGradeLevelRef as MaxGradeLevelId,
	Class.PrimaryTeacherRef as PrimaryTeacherRef,  
	Class.ChalkableDepartmentRef as DepartmentId
	
 From LessonPlan
 Join Announcement on Announcement.Id = LessonPlan.Id
 Join Class on Class.Id = LessonPlan.ClassRef
 Left Join Staff on Staff.Id = Class.PrimaryTeacherRef
 Go


 Create Type TLessonPlan as Table
 (
	Id int not null,
	Created dateTime2 not null,
	[State] int not null,
	Content nvarchar(max),
	[Title] nvarchar(max),
	ClassRef int not null,
	SchoolRef int not null,
	StartDate datetime2 null,
	EndDate datetime2,
	GalleryCategoryRef int,
	VisibleForStudent bit,

	PrimaryTeacherName nvarchar(max),
	PrimaryTeacherGender nvarchar(max),
	ClassName nvarchar(max),
	FullClassName nvarchar(max),
	MinGradeLevelId int,
	MaxGradeLevelId int,
	PrimaryTeacherRef int,
	DepartmentId uniqueidentifier,
	IsOwner bit,
	Complete bit,
	AllCount int
 )
 Go

 
 
Alter View [dbo].[vwAnnouncementQnA]
As
	Select
		ana.id as Id,
		ana.AnsweredTime as AnsweredTime,
		ana.QuestionTime as QuestionTime,
		ana.Question as Question,
		ana.Answer as Answer,
		ana.AnnouncementRef as AnnouncementRef,
		(case when ca.ClassRef is not null then ca.ClassRef else lp.ClassRef end) as ClassRef,
		aa.AdminRef as AdminRef,
		
		ana.[State] as [State],
		st.Id as AskerId,
		st.FirstName as AskerFirstName,
		st.LastName as AskerLastName,
		st.Gender as AskerGender,
		cast(3 as int) as AskerRoleRef,
		sts.SchoolRef as AskerSchoolRef,
		sf.Id as AnswererId,
		sf.FirstName as AnswererFirstName,
		sf.LastName as AnswererLastName,
		sf.Gender as AnswererGender,
		cast(2 as int) as AnswererRoleRef,
		sfs.SchoolRef as AnswererSchoolRef
	From
		AnnouncementQnA ana
		join Student st on st.Id = ana.AskerRef
		join StudentSchool sts on sts.StudentRef = st.Id
		left join LessonPlan lp on lp.Id = ana.AnnouncementRef
		left join ClassAnnouncement ca on ca.Id = ana.AnnouncementRef
		left join AdminAnnouncement aa on aa.Id = ana.AnnouncementRef
		left join Staff sf on sf.Id = ana.AnswererRef
		left join StaffSchool sfs on sfs.StaffRef = sf.Id
Go


Drop view vwAdminAnnouncement