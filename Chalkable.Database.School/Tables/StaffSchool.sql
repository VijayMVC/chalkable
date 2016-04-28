CREATE TABLE [dbo].[StaffSchool] (
    [StaffRef]  INT NOT NULL,
    [SchoolRef] INT NOT NULL,
    CONSTRAINT [PK_StaffSchool] PRIMARY KEY CLUSTERED ([StaffRef] ASC, [SchoolRef] ASC),
    CONSTRAINT [FK_StaffSchool_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [FK_StaffSchool_Staff] FOREIGN KEY ([StaffRef]) REFERENCES [dbo].[Staff] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [StaffSchool_Staff]
    ON [dbo].[StaffSchool]([StaffRef] ASC);

GO	

CREATE NONCLUSTERED INDEX IX_StaffSchool_SchoolRef
	ON dbo.StaffSchool( SchoolRef )
GO