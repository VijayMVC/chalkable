CREATE TYPE [dbo].[TApplicationInstallActionGradeLevel] AS TABLE (
    [Id]                  INT NOT NULL,
    [GradeLevelRef]       INT NOT NULL,
    [AppInstallActionRef] INT NOT NULL);

