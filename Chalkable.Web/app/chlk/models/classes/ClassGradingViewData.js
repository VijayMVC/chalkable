REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.MarkingPeriodId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassGradingViewData*/
    CLASS(
        'ClassGradingViewData', EXTENDS(chlk.models.classes.Class), [
            [ria.serialize.SerializeProperty('gradingpermp')],
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'gradingPerMp'
        ]);
});
