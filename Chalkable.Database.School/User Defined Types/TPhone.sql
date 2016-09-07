CREATE TYPE [dbo].[TPhone] AS TABLE (
    [PersonRef]      INT            NOT NULL,
    [Value]          NVARCHAR (256) NOT NULL,
    [Type]           INT            NOT NULL,
    [DigitOnlyValue] NVARCHAR (256) NOT NULL,
    [IsPrimary]      BIT            NOT NULL);

