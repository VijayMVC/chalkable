REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.ClassPersonGradingItem*/
    CLASS(
        UNSAFE, 'ClassPersonGradingItem', IMPLEMENTS(ria.serialize.IDeserializable), [

            Number, 'studentItemTypeAvg',

            chlk.models.announcement.AnnouncementType, 'announcementType',

            [ria.serialize.SerializeProperty('items')],
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'announcements',

            VOID, function deserialize(raw){
                this.studentItemTypeAvg = SJX.fromValue(raw.avg, Number);
                this.announcementType = SJX.fromDeserializable(raw.announcementtype, chlk.models.announcement.AnnouncementType);
                this.announcements = SJX.fromArrayOfDeserializables(raw.items, chlk.models.announcement.FeedAnnouncementViewData);
            }

        ]);
});
