REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.course.Course');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.MarkingPeriodId');


NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.Class*/
    CLASS(
        'Class', [
            chlk.models.course.Course, 'course',

            String, 'description',

            Boolean, 'disabled',

            [ria.serialize.SerializeProperty('gradelevel')],
            chlk.models.grading.GradeLevel, 'gradeLevel',

            chlk.models.id.ClassId, 'id',

            [ria.serialize.SerializeProperty('markingperiodsid')],
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',

            String, 'name',

            chlk.models.people.User, 'teacher',

            Boolean, 'pressed',

            Number, 'index'
        ]);
});
