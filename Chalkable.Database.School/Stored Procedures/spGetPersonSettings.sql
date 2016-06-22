CREATE Procedure [dbo].[spGetPersonSettings]
@personId int,
@schoolYearId int,
@classId int,
@keys TString ReadOnly
As

declare @all int = (select count(*) from @keys)

Select * From PersonSetting
Where
(@personId is null or  PersonRef = @personId)
And (@schoolYearId is null or SchoolYearRef = @schoolYearId)
And (@classId is null or ClassRef = @classId)
And ([Key] in(select value from @keys) or @all = 0 )