

CREATE Procedure [dbo].[spGetStandardParentsWithChilds]
@standardId int,
@classId int
As
Declare @parentsIds Table (id int)
Declare @children [TStandard]
Declare @parentId int = @standardId,
@currentStandardId int,
@subjectId int
Declare @faild bit = 0

Insert Into @children
Select * from [Standard]
Where
ParentStandardRef = @standardId and IsActive = 1

While @parentId is not null
Begin
Select
@currentStandardId = Id,
@parentId = ParentStandardRef,
@subjectId = StandardSubjectRef
From [Standard]
Where
Id = @parentId
and IsActive = 1

If @currentStandardId is null
Set @faild = 1

If @parentId is not null
Begin
Insert Into @children
Select * From [Standard]
Where
ParentStandardRef = @parentId
and IsActive = 1
End
Else
If @classId is not null
Begin
Insert Into @children
Select S.*
From Class as C
join ClassStandard as CS
on C.Id = CS.ClassRef or CS.ClassRef = C.CourseRef
join [Standard] as S
on S.Id = CS.StandardRef
Where
S.StandardSubjectRef = @subjectId
and S.IsActive = 1
and C.Id = @classId
End
Else
Insert Into @children
Select * From [Standard]
Where
StandardSubjectRef = @subjectId
and IsActive = 1

Insert Into @parentsIds
Values (@currentStandardId)
End

If @faild = 1
Begin
Delete From @parentsIds
Delete From @children
End
Select * From @parentsIds
Select * From @children