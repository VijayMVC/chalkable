CREATE TABLE [dbo].[GradingComment] (
    [Id]        INT           NOT NULL,
    [SchoolRef] INT           NOT NULL,
    [Code]      NVARCHAR (5)  NOT NULL,
    [Comment]   NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GradingComment_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [UQ_GradingComment_SchoolRef_Code] UNIQUE NONCLUSTERED ([SchoolRef] ASC, [Code] ASC)
);

