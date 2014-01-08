REQUIRE('chlk.models.announcement.ItemStandard');

NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AddStandardViewData*/
    CLASS('AddStandardViewData', [

        String, 'announcementTypeName',
        ArrayOf(chlk.models.announcement.ItemStandard), 'itemStandards',

        [[String, ArrayOf(chlk.models.announcement.ItemStandard)]],
        function $(announcementTypeName, itemStandards){
            BASE();
            this.setAnnouncementTypeName(announcementTypeName);
            this.setItemStandards(itemStandards);
        }
    ]);
});