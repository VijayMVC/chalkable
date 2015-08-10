CREATE TABLE [dbo].[ApplicationDistrictOption] (
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [DistrictRef]    UNIQUEIDENTIFIER NOT NULL,
    [Ban]            BIT              NOT NULL,
    CONSTRAINT [FK_ApplicationDistrictOption_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [FK_ApplicationDistrictOption_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id])
);

