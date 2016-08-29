REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');
REQUIRE('chlk.models.announcement.BaseStudentAnnouncementsViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortStudentAnnouncementsViewData*/
    CLASS(
        UNSAFE, 'ShortStudentAnnouncementsViewData',
                EXTENDS(chlk.models.announcement.BaseStudentAnnouncementsViewData.OF(chlk.models.announcement.ShortStudentAnnouncementViewData)),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.items = ria.serialize.SJX.fromArrayOfDeserializables(raw.items, chlk.models.announcement.ShortStudentAnnouncementViewData);
            },

            function $(items_){
                BASE();
                if(items_)
                    this.setItems(items_);
            }
        ]);
});
