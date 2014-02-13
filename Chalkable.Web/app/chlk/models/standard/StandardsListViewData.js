REQUIRE('chlk.models.standard.Standard');

NAMESPACE('chlk.models.standard', function(){
    /**@class chlk.models.standard.StandardsListViewData*/
    CLASS('StandardsListViewData', [

        ArrayOf(chlk.models.standard.Standard), 'itemStandards',

        String, 'description',

        [[String, ArrayOf(chlk.models.standard.Standard)]],
        function $(description, itemStandards){
            BASE();
            this.setAnnouncementTypeName(announcementTypeName);
            this.setDescription(description);
        }
    ]);
});