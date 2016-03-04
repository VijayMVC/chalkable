CREATE TABLE [dbo].[Address] (
    [Id]            INT             NOT NULL,
    [AddressNumber] NCHAR (15)      NOT NULL,
    [StreetNumber]  NVARCHAR (10)   NOT NULL,
    [AddressLine1]  NVARCHAR (75)   NOT NULL,
    [AddressLine2]  NVARCHAR (75)   NOT NULL,
    [City]          NVARCHAR (50)   NOT NULL,
    [State]         NVARCHAR (5)    NOT NULL,
    [PostalCode]    NCHAR (10)      NOT NULL,
    [Country]       NVARCHAR (60)   NOT NULL,
    [CountyId]      INT             NULL,
    [Latitude]      DECIMAL (10, 7) NULL,
    [Longitude]     DECIMAL (10, 7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO

CREATE TRIGGER AddressDeleteTrigger
ON Address
INSTEAD OF DELETE
AS
Update Person Set AddressRef = null where AddressRef in (Select Id From Deleted)
Delete From Address where Id in (Select Id From Deleted)

