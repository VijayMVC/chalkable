CREATE TYPE [dbo].[TStudentSchoolYear] AS TABLE (
    [SchoolYearRef]    INT NOT NULL,
    [GradeLevelRef]    INT NOT NULL,
    [StudentRef]       INT NOT NULL,
    [EnrollmentStatus] INT NOT NULL);

