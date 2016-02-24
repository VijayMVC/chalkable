CREATE TABLE [dbo].[School] (
    [Id]                              UNIQUEIDENTIFIER NOT NULL,
    [DistrictRef]                     UNIQUEIDENTIFIER NOT NULL,
    [Name]                            NVARCHAR (256)   NOT NULL,
    [LocalId]                         INT              NOT NULL,
    [IsChalkableEnabled]              BIT              NOT NULL,
    [StudyCenterEnabledTill]          DATETIME2 (7)    NULL,
    [IsLEEnabled]                     BIT              NOT NULL,
    [IsLESyncComplete]                BIT              NOT NULL,
    [IsMessagingDisabled]             BIT              NOT NULL,
    [StudentMessagingEnabled]         BIT              NOT NULL,
    [StudentToClassMessagingOnly]     BIT              NOT NULL,
    [TeacherToStudentMessaginEnabled] BIT              NOT NULL,
    [TeacherToClassMessagingOnly]     BIT              NOT NULL,
	[IsAssessmentEnabled]             BIT              NOT NULL
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_School_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id]),
    CONSTRAINT [UQ_School_DistrictRef_LocalId] UNIQUE NONCLUSTERED ([LocalId] ASC, [DistrictRef] ASC)
);



