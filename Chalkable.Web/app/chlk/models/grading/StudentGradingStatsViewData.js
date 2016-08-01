REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.StudentGradingStatsViewData*/
    CLASS('StudentGradingStatsViewData', [
        [ria.serialize.SerializeProperty('announcementgrades')],
        ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'announcementGrades',

        Number, 'grade',

        chlk.models.common.ChlkDate, 'date'
    ]);
});
