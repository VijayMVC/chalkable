CREATE TABLE [dbo].[UserSchool] (
    [UserRef]   INT NOT NULL,
    [SchoolRef] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([UserRef] ASC, [SchoolRef] ASC),
    CONSTRAINT [FK_UserSchool_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_UserSchool_School]
    ON [dbo].[UserSchool]([SchoolRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserSchool_User]
    ON [dbo].[UserSchool]([UserRef] ASC);

