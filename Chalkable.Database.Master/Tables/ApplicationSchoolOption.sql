CREATE TABLE [dbo].[ApplicationSchoolOption]
(
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL, 
    [SchoolRef] UNIQUEIDENTIFIER NOT NULL, 
    [Banned] BIT NOT NULL, 
    PRIMARY KEY ([SchoolRef], [ApplicationRef]),
	CONSTRAINT FK_ApplicationSchoolOption_Application FOREIGN KEY (ApplicationRef) References [Application](Id),
	CONSTRAINT FK_ApplicationSchoolOption_School FOREIGN KEY (SchoolRef) References [School](Id)
)
