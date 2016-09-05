REQUIRE('chlk.models.standard.StandardSubject');
REQUIRE('chlk.models.common.BaseAttachViewData');

//TODO: DELETE

NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AddStandardViewData*/
    CLASS('AddStandardViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

        ArrayOf(chlk.models.standard.StandardSubject), 'itemStandards',

        Array, 'standardIds',

        [[chlk.models.common.AttachOptionsViewData, ArrayOf(chlk.models.standard.StandardSubject), Array]],
        function $(options, itemStandards, standardIds){
            BASE(options);
            this.setItemStandards(itemStandards);
            this.setStandardIds(standardIds);
        }
    ]);
});