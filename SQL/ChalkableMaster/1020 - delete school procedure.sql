Create Procedure spDeleteSchool @id uniqueidentifier
as

BEGIN TRY
	BEGIN TRANsACTION
		delete from BackgroundTask where SchoolRef = @id
		delete from Fund where SchoolRef = @id
		delete from FundRequestRoleDistribution where FundRequestRef in (select id from FundRequest where SchoolRef = @id)
		delete from FundRequest where SchoolRef = @id		
		delete from SchoolUser where  SchoolRef = @id
		delete from SisSync where Id = @id
		delete from School where Id = @id
	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	if @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION
	END
	declare @errorMessage nvarchar(max) = ERROR_MESSAGE()
	declare @errorSeverity int = ERROR_SEVERITY()
	declare @errorState int = ERROR_STATE()	
	RAISERROR(@errorMessage, @errorSeverity, @errorState)
END CATCH
