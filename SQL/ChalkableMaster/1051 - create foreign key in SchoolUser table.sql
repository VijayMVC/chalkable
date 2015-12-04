Alter Table SchoolUser
add constraint FK_SchoolUser_School foreign key (SchoolRef, DistrictRef) references [School](LocalId, DistrictRef)
Go





