﻿CREATE TABLE [dbo].[MealType]
(
	[Id] INT IDENTITY (1,1) NOT NULL CONSTRAINT [CPK_MealType] PRIMARY KEY CLUSTERED ( [Id] ),
    [Code] VARCHAR(5) NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [Description] VARCHAR(255) NOT NULL CONSTRAINT [DF_MealType_Description] DEFAULT (''),
    [StateCode] VARCHAR(10) NOT NULL CONSTRAINT [DF_MealType_StateCode] DEFAULT(''),
    [SIFCode] VARCHAR(10) NOT NULL CONSTRAINT [DF_MealType_SIFCode] DEFAULT(''),
    [NCESCode] VARCHAR(10) NOT NULL CONSTRAINT [DF_MealType_NCESCode] DEFAULT(''),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_MealType_IsActive] DEFAULT (1),
    [IsSystem] BIT NOT NULL CONSTRAINT [DF_MealType_IsSystem] DEFAULT (0),
    CONSTRAINT [UNCC_MealType_Name] UNIQUE NONCLUSTERED ( [Name] ),
    CONSTRAINT [UNCC_MealType_Code] UNIQUE NONCLUSTERED ( [Code] )
)
