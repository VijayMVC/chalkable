REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementApplicationId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.announcement', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.announcement.StudentAnnouncementApplicationMeta*/
    CLASS(
        UNSAFE, 'StudentAnnouncementApplicationMeta', IMPLEMENTS(ria.serialize.IDeserializable),[

            VOID, function deserialize(raw) {
                this.announcementApplicationId = SJX.fromValue(raw.announcementapplicationid, chlk.models.id.AnnouncementApplicationId);
                this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
                this.text = SJX.fromValue(raw.text, String);
            },

            chlk.models.id.AnnouncementApplicationId, 'announcementApplicationId',

            chlk.models.id.SchoolPersonId, 'studentId',

            String, 'text'
    ]);
});