CREATE Procedure spGetStandardRelations
	@standardId UNIQUEIDENTIFIER
As

Declare @derivatives TGuid,
		@origins	 TGuid,
		@relatedDerivatives TGuid;

Insert Into @derivatives
	Select DerivativeRef From StandardRelation
	Where StandardRef = @standardId

Insert Into @origins
	Select StandardRef From StandardRelation
	Where DerivativeRef = @standardId

Insert Into @relatedDerivatives
	Select DerivativeRef From StandardRelation 
		join @relatedDerivatives as rd
			on StandardRelation.StandardRef = rd.Value

Select * From [Standard] join @origins orig on [Standard].Id = orig.Value

Select * From [Standard] join @derivatives deriv on [Standard].Id = deriv.Value

Select * From [Standard] join @relatedDerivatives relDeriv on [Standard].Id = relDeriv.Value