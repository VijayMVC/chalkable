alter table Class
add GradingScaleRef int null constraint FK_Class_GradingScale foreign key references GradingScale(Id)
go
