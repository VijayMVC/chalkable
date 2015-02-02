create table ABToCCMapping
(
	CCStandardRef uniqueidentifier not null constraint FK_ABToCCMapping_CommonCoreStandard foreign key references CommonCoreStandard(Id),
	AcademicBenchmarkId uniqueidentifier not null
)

alter table ABToCCMapping
add constraint PK_ABToCCMappingID primary key (AcademicBenchmarkId, CCStandardRef)
go

