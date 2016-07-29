CREATE TABLE [dbo].[SystemSetting] (
    [Category] NVARCHAR (25)  NOT NULL,
    [Setting]  NVARCHAR (50)  NOT NULL,
    [Value]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_SYSTEM_SETTING] PRIMARY KEY CLUSTERED ([Category] ASC, [Setting] ASC)
);

