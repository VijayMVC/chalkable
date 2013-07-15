alter table MarkingPeriodClass
add constraint UQ_MarkingPeriodClass_ClassRef_MarkingPeriodRef unique(ClassRef, MarkingPeriodRef)
go
alter table ClassPerson
add constraint QU_ClassPerson_PersonRef_ClassRef unique(PersonRef, ClassRef)
go
alter table ClassPeriod
add constraint QU_ClassPeriod_ClassRef_PeriodRef unique(ClassRef, PeriodRef)
go
alter table [Date]
add constraint QU_Date_DateTime unique([DateTime])
go
