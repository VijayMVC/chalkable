Create Procedure spGetPersonSettings
@personId int,
@keys TString ReadOnly
As

Select * From PersonSetting 
Where 
	PersonRef = @personId
	And [Key] in(select value from @keys)

Go