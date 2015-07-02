REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');
REQUIRE('chlk.models.id.AnnouncementId');


NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AnnouncementAttributeListViewData*/
    CLASS('AnnouncementAttributeListViewData',[
        ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData), 'announcementAttributes',
        Boolean, 'readOnly',

        chlk.models.id.AnnouncementId, 'announcementId',

        Boolean, function isEmpty(){
            return (this.getAnnouncementAttributes() || []).length == 0;
        }
    ]);
});