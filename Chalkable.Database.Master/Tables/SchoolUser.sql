CREATE TABLE [dbo].[SchoolUser] (
    [SchoolRef]   INT              NOT NULL,
    [UserRef]     INT              NOT NULL,
    [DistrictRef] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SchoolUser] PRIMARY KEY CLUSTERED ([SchoolRef] ASC, [UserRef] ASC, [DistrictRef] ASC),
    CONSTRAINT [FK_SchoolUser_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id])
);

