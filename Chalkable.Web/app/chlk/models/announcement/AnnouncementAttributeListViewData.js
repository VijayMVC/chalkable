REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');
REQUIRE('chlk.models.id.AnnouncementId');


NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AnnouncementAttributeListViewData*/
    CLASS('AnnouncementAttributeListViewData',[
        ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData), 'announcementAttributes',
        Boolean, 'readOnly',

        chlk.models.id.AnnouncementId, 'announcementId',

        chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

        Boolean, function isEmpty(){
            return (this.getAnnouncementAttributes() || []).length == 0;
        },

        Object, function getPostData(){

            var res = [];
            var attrs = (this.getAnnouncementAttributes() || []);

            attrs.forEach(function(item){
               res.push(item.getPostData());
            });


            return res;

        },

        String, function toJSON(){
            return JSON.stringify(this.getPostData());
        }

    ]);
});