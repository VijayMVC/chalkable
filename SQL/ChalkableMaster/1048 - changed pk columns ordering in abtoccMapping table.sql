Alter Table AbToccMapping
drop constraint PK_AbToCCMappingID
Go

Alter Table AbToccMapping
add constraint PK_AbToCCMappingID primary key (CCStandardRef, AcademicBenchmarkId)
Go