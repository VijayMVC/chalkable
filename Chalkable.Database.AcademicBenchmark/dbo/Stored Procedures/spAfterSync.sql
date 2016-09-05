Create Procedure spAfterSync
As
	ALTER TABLE [Standard] CHECK CONSTRAINT FK_Standard_Standard;