REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.Announcement');

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
            ArrayOf(chlk.models.announcement.Announcement), 'announcements'

        ]);
});
