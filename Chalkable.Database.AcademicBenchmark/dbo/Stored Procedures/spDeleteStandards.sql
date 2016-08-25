CREATE Procedure spDeleteStandards
	@standardIds TGuid ReadOnly
As

Update [Standard]
Set ParentRef = NULL
Where Id in(select * from @standardIds)

Delete From [StandardRelation]
Where 
	StandardRef in(select * From @standardIds)
	Or DerivativeRef in(select * From @standardIds)

Delete From [Standard]
Where Id in(select * from @standardIds)