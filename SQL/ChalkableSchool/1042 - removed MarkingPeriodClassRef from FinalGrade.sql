alter table FinalGrade
drop constraint FK_FinalGrade_MarkingPeriodClass
go
alter table FinalGrade
drop column MarkingPeriodClassRef
go
alter table FinalGrade
add constraint FK_FinalGrade_MarkingPeriodClass foreign key (Id) references MarkingPeriodClass(Id)
go
