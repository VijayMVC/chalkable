CREATE TYPE [dbo].[TSchool] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[DistrictRef] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[LocalId] [int] NOT NULL,
	[IsChalkableEnabled] [bit] NOT NULL,	
	[IsLEEnabled] [bit] NOT NULL,
	[IsLESyncComplete] [bit] NOT NULL,
	[StudyCenterEnabledTill] [datetime2](7) NULL,	
	[IsMessagingDisabled] [bit] NOT NULL,
	[IsAssessmentEnabled] [bit] NOT NULL,
	[StudentMessagingEnabled] [bit] NOT NULL,
	[StudentToClassMessagingOnly] [bit] NOT NULL,
	[TeacherToStudentMessaginEnabled] [bit] NOT NULL,
	[TeacherToClassMessagingOnly] [bit] NOT NULL	
)
