If (Object_Id('[dbo].[Fund]') Is Not Null)
Begin
	Drop Table [dbo].[Fund]
End

If (Object_Id('[dbo].[FundRequest]') Is Not Null)
Begin
	Drop Table [dbo].[FundRequest]
End

If (Object_Id('[dbo].[FundRequestRoleDistribution]') Is Not Null)
Begin
	Drop Table [dbo].[FundRequestRoleDistribution]
End

If (Object_Id('[dbo].[ApplicationRating]') Is Not Null)
Begin
	Drop Table [dbo].[ApplicationRating]
End

If (Object_Id('[dbo].[ApplicationDistrictOption]') Is Not Null)
Begin
	Drop Table [dbo].[ApplicationDistrictOption]
End