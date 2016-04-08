CREATE TYPE [dbo].[TAddress] AS TABLE (
    [Id]            INT             NOT NULL,
    [AddressNumber] NCHAR (15)      NOT NULL,
    [StreetNumber]  NVARCHAR (20)   NOT NULL,
    [AddressLine1]  NVARCHAR (150)  NOT NULL,
    [AddressLine2]  NVARCHAR (150)  NOT NULL,
    [City]          NVARCHAR (100)  NOT NULL,
    [State]         NVARCHAR (10)   NOT NULL,
    [PostalCode]    NCHAR (10)      NOT NULL,
    [Country]       NVARCHAR (120)  NOT NULL,
    [CountyId]      INT             NULL,
    [Latitude]      DECIMAL (10, 7) NULL,
    [Longitude]     DECIMAL (10, 7) NULL);

