USE [83009FE9-8594-4E33-A09E-1EF4F81D0E8D]
GO

/****** Object:  StoredProcedure [dbo].[spGetPersonSettings]    Script Date: 9/29/2015 9:54:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Alter Procedure [dbo].[spGetPersonSettings]
@personId int,
@schoolYearId int,
@keys TString ReadOnly
As

Select [Key], [Value] From PersonSetting 
Where 
	PersonRef = @personId
	And SchoolYearRef = @schoolYearId
	And ([Key] in(select value from @keys) or not exists(select * from @keys))


GO