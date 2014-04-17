REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementTypeGradingId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingSummaryGridSubmitViewData*/
    CLASS(
        'GradingSummaryGridSubmitViewData', [
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            chlk.models.id.StandardId, 'standardId',

            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',

            chlk.models.id.ClassId, 'classId',

            Boolean, 'autoUpdate',

            Boolean, 'notCalculateGrid'
        ]);
});