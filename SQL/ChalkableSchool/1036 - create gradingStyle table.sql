create table GradingStyle
(
	Id uniqueidentifier not null primary key,
	GradingStyleValue int not null, 
	MaxValue int not null,
	StyledValue int not null
)
go
