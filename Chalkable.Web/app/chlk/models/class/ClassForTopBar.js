REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StaffCourseId');
REQUIRE('chlk.models.id.MarkingPeriodId');


NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassForTopBar*/
    CLASS(
        'ClassForTopBar', [
            [ria.serialize.SerializeProperty('courseinfoid')],
            chlk.models.id.CourseId, 'courseInfoId',

            [ria.serialize.SerializeProperty('coursetitle')],
            String, 'courseTitle',

            String, 'description',

            Boolean, 'disabled',

            [ria.serialize.SerializeProperty('gradelevelid')],
            Number, 'gradeLevelId',

            [ria.serialize.SerializeProperty('gradelevelname')],
            String, 'gradeLevelName',

            chlk.models.id.ClassId, 'id',

            [ria.serialize.SerializeProperty('markingperiodsid')],
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',

            String, 'name',

            [ria.serialize.SerializeProperty('staffcourseid')],
            chlk.models.id.StaffCourseId, 'staffCourseId',

            [ria.serialize.SerializeProperty('teacherid')],
            chlk.models.id.SchoolPersonId, 'teacherId',

            [ria.serialize.SerializeProperty('teachername')],
            String, 'teacherName',

            Boolean, 'pressed',

            Number, 'index'
        ]);
});
