alter table School
add constraint UQ_School_DistrictRef_LocalId unique (LocalId, DistrictRef)
