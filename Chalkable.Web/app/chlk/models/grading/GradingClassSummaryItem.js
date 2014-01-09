REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItem*/
    CLASS(
        'GradingClassSummaryItem', [
            chlk.models.announcement.ClassAnnouncementType, 'type',
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'announcements',
            Number, 'percent',
            Number, 'avg',
            Number, 'index',
            chlk.models.id.ClassId, 'classId'
        ]);
});
