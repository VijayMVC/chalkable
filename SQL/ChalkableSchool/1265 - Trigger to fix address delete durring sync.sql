IF EXISTS (SELECT *    FROM sys.objects    WHERE [type] = 'TR' AND [name] = 'AddressDeleteTrigger '    )
    DROP TRIGGER AddressDeleteTrigger ;
GO

CREATE TRIGGER AddressDeleteTrigger 
ON Address
INSTEAD OF DELETE
AS
	Update Person Set AddressRef = null where AddressRef in (Select Id From Deleted)
    Delete From Address where Id in (Select Id From Deleted)

