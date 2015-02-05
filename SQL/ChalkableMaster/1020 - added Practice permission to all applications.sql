declare @appId nvarchar(max) = (select value from Preference where [key] = 'practice_application_id')

insert into ApplicationPermission 
select newID(), @appId, 8