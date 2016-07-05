CREATE TABLE [dbo].[Student] (
    [Id]					  INT            NOT NULL,
    [FirstName]				  NVARCHAR (255) NOT NULL,
    [LastName]				  NVARCHAR (255) NOT NULL,
    [BirthDate]				  DATETIME2 (7)  NULL,
    [Gender]				  NVARCHAR (255) NULL,
    [HasMedicalAlert]		  BIT            NOT NULL,
    [IsAllowedInetAccess]	  BIT            NOT NULL,
    [SpecialInstructions]	  NVARCHAR (MAX) NOT NULL,
    [SpEdStatus]			  NVARCHAR (256) NULL,
    [PhotoModifiedDate]       DATETIME2 (7)  NULL,
    [UserId]				  INT            NOT NULL,
	[IsHispanic]			  BIT			 NOT NULL DEFAULT 0,
	[IEPBeginDate]			  DATETIME2(7)	 NULL,
	[IEPEndDate]			  DATETIME2(7)	 NULL,
	[Section504Qualification] NVARCHAR(128)  NULL,
	[GenderDescriptor]		  NVARCHAR(128)  NULL,
	[IsHomeless]	    	  BIT			 NOT NULL DEFAULT 0,
	[IsImmigrant]			  BIT			 NOT NULL DEFAULT 0,
	[LimitedEnglishRef]		  INT			 NULL,
	[IsForeignExchange]		  BIT			 NOT NULL DEFAULT 0,
	[StateIdNumber]		      NVARCHAR(128)  NULL,
	[AltStudentNumber]	      NVARCHAR(128)  NULL,
	[StudentNumber]		      NVARCHAR(128)	 NULL,
	[OriginalEnrollmentDate]  DATETIME2(7)	 NULL,
	[Section504Qualification] NVARCHAR(128) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Student_Person] FOREIGN KEY ([Id]) REFERENCES [dbo].[Person] ([Id])
);

/* public string GenderDescriptor { get; set; }
        public string Section504Qualification { get; set; }
        public bool IsHomeless { get; set; }
        public bool IsImmigrant { get; set; }
        public int? LimitedEnglishRef { get; set; }
        public bool IsForeignExchange { get; set; }
        public string StateIdNumber { get; set; }
        public string AltStudentNumber { get; set; }
        public string StudentNumber { get; set; }
        public DateTime? OriginalEnrollmentDate { get; set; }*/