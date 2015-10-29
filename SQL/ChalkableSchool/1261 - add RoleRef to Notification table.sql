Alter Table Notification
Add RoleRef int null
Go

Update n1 
Set n1.RoleRef = p.RoleRef
From Notification n1
Join (
		Select Id, 3 as RoleRef
		From  Student
		Union 
		Select Id, 2 as RoleRef
		From Staff
	 ) p on p.Id = n1.PersonRef

Update Notification
Set RoleRef = 10
Where AnnouncementRef is not null And RoleRef = 2 and AnnouncementRef in (Select Id From AdminAnnouncement) 
Go

Alter Table Notification
Alter Column RoleRef int not null
Go