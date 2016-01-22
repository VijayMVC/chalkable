REQUIRE('chlk.models.standard.CCStandardCategory');
REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardViewData*/
    CLASS('AddCCStandardViewData', [

        ArrayOf(chlk.models.standard.CCStandardCategory), 'items',

        String, 'requestId',

        Array, 'standardsIds',

        [[String, ArrayOf(chlk.models.standard.CCStandardCategory), Array]],
        function $(requestId,  itemStandards, standardsIds){
            BASE();
            if(requestId)
                this.setRequestId(requestId);
            if(itemStandards)
                this.setItems(itemStandards);
            if(standardsIds)
                this.setStandardsIds(standardsIds);
        }
    ]);
});
