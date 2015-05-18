REQUIRE('chlk.models.id.AnnouncementAttributeId');

NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AnnouncementAttributeViewData*/
    CLASS('AnnouncementAttributeViewData',[
        chlk.models.id.AnnouncementAttributeId, 'id',

        String, 'code',

        String, 'name',

        String, 'description'
    ]);
});