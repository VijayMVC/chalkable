
Create View vwIncomeMessage
As

select
	pm.[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[Read],
	[DeletedByRecipient],
	p.Id as SenderId,
	p.FirstName as SenderFirstName,
	p.LastName as SenderLastName,
	p.RoleRef as SenderRoleRef,
	p.Gender as SenderGender,
	pmr.RecipientRef
from
	PrivateMessage pm 
		join PrivateMessageRecipient pmr
			on pm.Id = pmr.PrivateMessageRef
join (select
[Id],
[FirstName],
[LastName],
3 as [RoleRef],
[Gender]
From
Student
Union
select
[Id],
[FirstName],
[LastName],
2 as [RoleRef],
[Gender]
From
Staff
) p
on p.Id = pm.FromPersonRef