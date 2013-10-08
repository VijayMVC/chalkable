sp_rename 'CourseInfo', 'Course'
go
sp_rename 'FK_Class_CourseInfoRef', 'FK_Class_Course', 'OBJECT'
go
sp_rename 'Class.CourseInfoRef', 'Class.CourseRef', 'COLUMN'
go