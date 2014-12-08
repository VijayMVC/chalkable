REQUIRE('chlk.models.standard.CCStandardCategory');
REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardViewData*/
    CLASS('AddCCStandardViewData', [

        ArrayOf(chlk.models.standard.CCStandardCategory), 'items',

        chlk.models.id.AppId, 'applicationId',

        Array, 'standardCodes',

        [[chlk.models.id.AppId, ArrayOf(chlk.models.standard.CCStandardCategory), Array]],
        function $(applicationId,  itemStandards, standardCodes){
            BASE();
            if(applicationId)
                this.setApplicationId(applicationId);
            if(itemStandards)
                this.setItems(itemStandards);
            if(standardCodes)
                this.setStandardCodes(standardCodes);
        }
    ]);
});