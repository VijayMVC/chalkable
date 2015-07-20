REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.ClassPersonGradingItem*/
    CLASS(
        'ClassPersonGradingItem', EXTENDS(chlk.models.announcement.AnnouncementType), [

            [ria.serialize.SerializeProperty('studentitemtypeavg')],
            Number, 'studentItemTypeAvg',

            [ria.serialize.SerializeProperty('classitemtypeavg')],
            Number, 'classItemTypeAvg',

            [ria.serialize.SerializeProperty('items')],
            ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), 'announcements'

        ]);
});
