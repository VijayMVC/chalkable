CREATE TYPE [dbo].[TSystemSetting] AS TABLE (
    [Category] NVARCHAR (25)  NOT NULL,
    [Setting]  NVARCHAR (50)  NOT NULL,
    [Value]    NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Category] ASC, [Setting] ASC));

