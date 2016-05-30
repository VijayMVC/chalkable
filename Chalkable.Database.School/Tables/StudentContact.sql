CREATE TABLE [dbo].[StudentContact] (
    [StudentRef]             INT            NOT NULL,
    [ContactRef]             INT            NOT NULL,
    [ContactRelationshipRef] INT            NOT NULL,
    [Description]            NVARCHAR (255) NOT NULL,
    [ReceivesMailings]       BIT            NOT NULL,
    [CanPickUp]              BIT            NOT NULL,
    [IsFamilyMember]         BIT            NOT NULL,
    [IsCustodian]            BIT            NOT NULL,
    [IsEmergencyContact]     BIT            NOT NULL,
    [IsResponsibleForBill]   BIT            NOT NULL,
    [ReceivesBill]           BIT            NOT NULL,
    [StudentVisibleInHome]   BIT            NOT NULL,
    CONSTRAINT [PK_StudentContact] PRIMARY KEY CLUSTERED ([StudentRef] ASC, [ContactRef] ASC),
    CONSTRAINT [FK_StudentContact_ContactRelationship] FOREIGN KEY ([ContactRelationshipRef]) REFERENCES [dbo].[ContactRelationship] ([Id]),
    CONSTRAINT [FK_StudentContact_Person] FOREIGN KEY ([ContactRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_StudentContact_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_StudentContact_ContactRef
	ON dbo.StudentContact( ContactRef )
GO


CREATE NONCLUSTERED INDEX IX_StudentContact_ContactRelationshipRef
	ON dbo.StudentContact( ContactRelationshipRef )
GO