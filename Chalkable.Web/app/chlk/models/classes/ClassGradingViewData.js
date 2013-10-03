REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.course.Course');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.MarkingPeriodId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassGradingViewData*/
    CLASS(
        'ClassGradingViewData', [
            [ria.serialize.SerializeProperty('gradingpermp')],
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'gradingPerMp',

            chlk.models.id.ClassId, 'id',

            String, 'name',

            String, 'description',

            chlk.models.course.Course, 'course',

            [ria.serialize.SerializeProperty('gradelevel')],
            chlk.models.grading.GradeLevel, 'gradeLevel',

            chlk.models.people.User, 'teacher',

            [ria.serialize.SerializeProperty('markingperiodsid')],
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId'
        ]);
});
