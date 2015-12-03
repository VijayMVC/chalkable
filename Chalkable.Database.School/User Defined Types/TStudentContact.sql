CREATE TYPE [dbo].[TStudentContact] AS TABLE (
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
    [StudentVisibleInHome]   BIT            NOT NULL);

