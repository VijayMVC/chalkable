REQUIRE('chlk.models.standard.StandardSubject');

NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AddStandardViewData*/
    CLASS('AddStandardViewData', [

        String, 'announcementTypeName',

        ArrayOf(chlk.models.standard.StandardSubject), 'itemStandards',

        [[String, ArrayOf(chlk.models.standard.StandardSubject)]],
        function $(announcementTypeName, itemStandards){
            BASE();
            this.setAnnouncementTypeName(announcementTypeName);
            this.setItemStandards(itemStandards);
        }
    ]);
});