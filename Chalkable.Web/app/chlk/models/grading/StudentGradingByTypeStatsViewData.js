REQUIRE('chlk.models.grading.StudentGradingStatsViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.StudentGradingByTypeStatsViewData*/
    CLASS('StudentGradingByTypeStatsViewData', [
        [ria.serialize.SerializeProperty('classannouncementtypeid')],
        Number, 'classAnnouncementTypeId',

        [ria.serialize.SerializeProperty('classannouncementtypename')],
        String, 'classAnnouncementTypeName',

        [ria.serialize.SerializeProperty('studentgradingstats')],
        ArrayOf(chlk.models.grading.StudentGradingStatsViewData), 'studentGradingStats'
    ]);
});
