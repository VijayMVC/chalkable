If (Object_Id('[dbo].[ApplicationInstallActionClasses]') Is Not Null)
Begin
	Drop Table [dbo].[ApplicationInstallActionClasses]
End

If (Object_Id('[dbo].[ApplicationInstall]') Is Not Null)
Begin
	Drop Table [dbo].[ApplicationInstall]
End

If (Object_Id('[dbo].[ApplicationInstallAction]') Is Not Null)
Begin
	Drop Table [dbo].[ApplicationInstallAction]
End

If (Object_Id('[dbo].[ApplicationBanHistory]') Is Not Null)
Begin
	Drop Table [dbo].[ApplicationBanHistory]
End