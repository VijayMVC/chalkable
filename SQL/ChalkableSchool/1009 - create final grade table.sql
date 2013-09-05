CREATE TABLE [dbo].[FinalGrade](
	[Id] uniqueidentifier primary key NOT NULL,
	[MarkingPeriodClassRef] uniqueidentifier NOT NULL constraint [FK_FinalGrade_MarkingPeriodClass] foreign key references MarkingPeriodClass(Id),
	[Status] [int] NOT NULL,
	[ParticipationPercent] [int] NOT NULL,
	[Discipline] [int] NOT NULL,
	[DropLowestDiscipline] [bit] NOT NULL,
	[Attendance] [int] NOT NULL,
	[DropLowestAttendance] [bit] NOT NULL,
	[GradingStyle] [int] NOT NULL,
)
GO
