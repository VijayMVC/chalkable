CREATE TYPE [dbo].[TInfraction] AS TABLE (
    [Id]                 INT           NOT NULL,
    [Code]               VARCHAR (5)   NOT NULL,
    [Name]               VARCHAR (50)  NOT NULL,
    [Description]        VARCHAR (255) NOT NULL,
    [Demerits]           TINYINT       NOT NULL,
    [StateCode]          VARCHAR (10)  NOT NULL,
    [SIFCode]            VARCHAR (10)  NOT NULL,
    [NCESCode]           VARCHAR (10)  NOT NULL,
    [IsActive]           BIT           NOT NULL,
    [IsSystem]           BIT           NOT NULL,
    [VisibleInClassroom] BIT           NOT NULL);



