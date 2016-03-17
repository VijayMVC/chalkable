CREATE TYPE [dbo].[TSchoolUser] AS TABLE (
    [SchoolRef]   INT              NOT NULL,
    [UserRef]     INT              NOT NULL,
    [DistrictRef] UNIQUEIDENTIFIER NOT NULL);

