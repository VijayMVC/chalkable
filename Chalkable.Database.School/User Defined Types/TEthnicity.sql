﻿CREATE TYPE [dbo].[TEthnicity] AS TABLE
(
	[Id] INT NOT NULL, 
    [Code] NVARCHAR(100) NULL, 
    [Name] NVARCHAR(100) NULL, 
    [Description] NVARCHAR(256) NULL, 
    [StateCode] NVARCHAR(100) NULL, 
    [SIFCode] NVARCHAR(100) NULL, 
    [NCESCode] NVARCHAR(100) NULL, 
    [IsActive] BIT NOT NULL, 
    [IsSystem] BIT NOT NULL
)
