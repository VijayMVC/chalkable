CREATE TYPE [dbo].[TApplicationInstallActionDepartment] AS TABLE (
    [Id]                  INT              NOT NULL,
    [DepartmentRef]       UNIQUEIDENTIFIER NOT NULL,
    [AppInstallActionRef] INT              NOT NULL);

