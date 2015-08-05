REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.ClassPersonGradingItem*/
    CLASS(
        'ClassPersonGradingItem', [

            [ria.serialize.SerializeProperty('studentitemtypeavg')],
            Number, 'studentItemTypeAvg',

            [ria.serialize.SerializeProperty('announcementtype')],
            chlk.models.announcement.AnnouncementType, 'announcementType',

            [ria.serialize.SerializeProperty('items')],
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'announcements'

        ]);
});
