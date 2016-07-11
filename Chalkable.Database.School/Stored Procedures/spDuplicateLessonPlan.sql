﻿
Create Procedure spDuplicateLessonPlan @lessonPlanId int, @classIds TInt32 readonly, @created DateTime2
As
Begin Transaction
-- only teacher can modify lesson plan
Declare @callerRole int =  2, @schoolYearId int
Declare @content nvarchar(max), @title nvarchar(max), 
		@discussionEnabled bit, @previewComments bit, @requireComments bit,
		@startDate datetime2, @endDate datetime2, 
		@visibleForStudent bit, @inGallery bit, @GalleryOwnerRef int

Select Top 1
@content = Content,
@startDate = StartDate,
@endDate = EndDate,
@visibleForStudent = VisibleForStudent ,
@title = Title,
@schoolYearId = SchoolYearRef,
@inGallery = InGallery,
@galleryOwnerRef = GalleryOwnerRef,
@discussionEnabled = DiscussionEnabled,
@previewComments = PreviewCommentsEnabled,
@requireComments = RequireCommentsEnabled
From vwLessonPlan
Where Id = @lessonPlanId

-- inserting lesson plan
Declare @newLpIds TInt32
Declare @classId int, @newLpId int
Declare @classIdsCount int = (select count(*) from @classIds)

Declare AnnouncementCursor Cursor For
Select id.value
From @classIds id

Open AnnouncementCursor
Fetch Next From AnnouncementCursor
Into @classId

While @@FETCH_STATUS = 0
Begin
Insert Into Announcement
Values(@content, @created, 1, @title, @discussionEnabled, @previewComments, @requireComments)

set @newLpId = Scope_Identity()

Insert Into LessonPlan
values (@newLpId, @startDate, @endDate, @classId, null, null, @visibleForStudent, @schoolYearId, @inGallery, @galleryOwnerRef)

Insert Into @newLpIds
values(@newLpId)

Fetch Next From AnnouncementCursor
Into @classId

End
Close AnnouncementCursor;
Deallocate AnnouncementCursor;


Insert Into AnnouncementStandard
Select  StandardRef, id.value
From @newLpIds id
Cross Join AnnouncementStandard
Where AnnouncementStandard.AnnouncementRef = @lessonPlanId


-- get insertedIds
Select id.value as LessonPlanId from @newLpIds id

Commit Transaction