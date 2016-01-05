CREATE TYPE [dbo].[TGradingComment] AS TABLE (
    [Id]        INT           NOT NULL,
    [SchoolRef] INT           NOT NULL,
    [Code]      NVARCHAR (5)  NOT NULL,
    [Comment]   NVARCHAR (50) NOT NULL);

