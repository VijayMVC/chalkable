create table AttendanceMonth
(
	Id int not null primary key,
	SchoolYearRef int not null constraint FK_AttendanceMonth_SchoolYear foreign key references SchoolYear(Id),
	Name nvarchar(30) not null,
	StartDate datetime2 not null,
	EndDate datetime2 not null,
	EndTime datetime2 not null,
	IsLockedAttendance bit not null, 
	IsLockedDiscipline bit not null
)
go


create table GradedItem
(
	Id int not null primary key,
	GradingPeriodRef int not null foreign key references GradingPeriod(Id),
	Name nvarchar(20) not null, 
	[Description] nvarchar(255) not null, 
	AlphaOnly bit not null,
	AppearsOnReportCard bit not null,
	DetGradePoints bit not null,
	DetGradeCredit bit not null,
	PostToTranscript bit not null, 
	AllowExemption bit not null,
	DisplayAsAvgInGradebook bit not null,
	PostRoundedAverage bit not null,
	Sequence int not null,
	AveragingRule nchar(1) not null 
)
go
