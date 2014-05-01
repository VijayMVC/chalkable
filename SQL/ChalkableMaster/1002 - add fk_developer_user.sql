alter table Developer
add constraint FK_Developer_User foreign key([Id]) references [User](Id)
go