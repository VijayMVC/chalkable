REQUIRE('chlk.models.standard.CCStandardCategory');
REQUIRE('chlk.models.id.AppId');


NAMESPACE('chlk.models.standard', function(){

    /**@class chlk.models.standard.AddCCStandardViewData*/
    CLASS('AddCCStandardViewData', [

        ArrayOf(chlk.models.standard.CCStandardCategory), 'items',

        String, 'requestId',

        Array, 'standardsIds',

        Boolean, 'onlyLeafs',

        [[String, ArrayOf(chlk.models.standard.CCStandardCategory), Array, Boolean]],
        function $(requestId,  itemStandards, standardsIds, onlyLeafs_){
            BASE();
            if(requestId)
                this.setRequestId(requestId);
            if(itemStandards)
                this.setItems(itemStandards);
            if(standardsIds)
                this.setStandardsIds(standardsIds);
            if(onlyLeafs_)
                this.setOnlyLeafs(onlyLeafs_);
        }
    ]);
});
