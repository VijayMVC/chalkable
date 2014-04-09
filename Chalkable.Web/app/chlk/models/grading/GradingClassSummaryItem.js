REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItem*/
    CLASS(
        'GradingClassSummaryItem', [
            chlk.models.common.NameId, 'type',

            ArrayOf(chlk.models.announcement.BaseAnnouncementViewData), 'announcements',

            Number, 'percent',

            Number, 'avg',

            Number, 'index',

            chlk.models.id.ClassId, 'classId'
        ]);
});
