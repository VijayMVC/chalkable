
create procedure [dbo].[spDeleteDistrictData] 
	@districtId uniqueidentifier
as
begin transaction
delete from schooluser where districtref = @districtId
delete from school where districtref = @districtId
delete from userlogininfo where id in (select id from [user] where districtref = @districtId)
delete from [user] where districtref = @districtId
commit