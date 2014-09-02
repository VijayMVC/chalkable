CREATE TABLE [dbo].[ScheduledTimeSlot](
	[BellScheduleId] [int] NOT NULL,
	[TimeSlotId] [int] NOT NULL,
	[StartTime] [int] NULL,
	[EndTime] [int] NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDailyAttendancePeriod] [bit] NOT NULL,
	[DistrictGuid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [CPK_ScheduledTimeSlot] PRIMARY KEY CLUSTERED 
(
	[BellScheduleID] ASC,
	[TimeSlotID] ASC
)
) ON [PRIMARY]

GO

CREATE TYPE [dbo].[TScheduledTimeSlot] AS TABLE (
	[BellScheduleId] [int] NOT NULL,
	[TimeSlotId] [int] NOT NULL,
	[StartTime] [int] NULL,
	[EndTime] [int] NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDailyAttendancePeriod] [bit] NOT NULL,
	[DistrictGuid] [uniqueidentifier] NOT NULL
)

GO

Alter Table Person
	Add SisStudentUserId int
GO

Alter Table Person
	Add SisStaffUserId int
GO

Drop Type [TPerson]
GO

CREATE TYPE [dbo].[TPerson] AS TABLE (
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[LastPasswordReset] [datetime2](7) NULL,
	[FirstLoginDate] [datetime2](7) NULL,
	[LastMailNotification] [datetime2](7) NULL,
	[Email] [nvarchar](256) NOT NULL,
	[AddressRef] [int] NULL,
	[HasMedicalAlert] [bit] NOT NULL,
	[IsAllowedInetAccess] [bit] NOT NULL,
	[SpecialInstructions] [nvarchar](1024) NOT NULL,
	[SpEdStatus] [nvarchar](256) NULL,
	[PhotoModifiedDate] [datetime2](7) NULL,
	[SisStudentUserId] int NULL,
	[SisStaffUserId] int NULL
)

GO