CREATE Procedure [dbo].[spGetPersonSettings]
@personId int,
@schoolYearId int,
@keys TString ReadOnly
As

declare @all int = (select count(*) from @keys)

Select [Key], [Value] From PersonSetting
Where
PersonRef = @personId
And SchoolYearRef = @schoolYearId
And ([Key] in(select value from @keys) or @all = 0 )