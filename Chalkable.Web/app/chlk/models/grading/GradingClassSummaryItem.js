REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItem*/
    CLASS(
        'GradingClassSummaryItem', [
            chlk.models.announcement.AnnouncementType, 'type',
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',
            Number, 'percent',
            Number, 'avg',
            chlk.models.id.ClassId, 'classId'
        ]);
});
