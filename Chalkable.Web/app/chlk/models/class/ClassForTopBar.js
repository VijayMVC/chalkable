REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.class.CourseId*/
    IDENTIFIER('CourseId');

    /** @class chlk.models.class.MarkingPeriodId*/
    IDENTIFIER('MarkingPeriodId');

    /** @class chlk.models.class.StaffCourseId*/
    IDENTIFIER('StaffCourseId');

    /** @class chlk.models.class.ClassForTopBar*/
    CLASS(
        'ClassForTopBar', [
            [ria.serialize.SerializeProperty('courseinfoid')],
            chlk.models.class.CourseId, 'courseInfoId',

            [ria.serialize.SerializeProperty('coursetitle')],
            String, 'courseTitle',

            String, 'description',

            [ria.serialize.SerializeProperty('gradelevelid')],
            Number, 'gradeLevelId',

            [ria.serialize.SerializeProperty('gradelevelname')],
            String, 'gradeLevelName',

            chlk.models.class.ClassId, 'id',

            [ria.serialize.SerializeProperty('markingperiodsid')],
            ArrayOf(chlk.models.class.MarkingPeriodId), 'markingPeriodsId',

            String, 'name',

            [ria.serialize.SerializeProperty('staffcourseid')],
            chlk.models.class.StaffCourseId, 'staffCourseId',

            [ria.serialize.SerializeProperty('teacherid')],
            chlk.models.people.SchoolPersonId, 'teacherId',

            [ria.serialize.SerializeProperty('teachername')],
            String, 'teacherName',

            Boolean, 'pressed',

            Number, 'index'
        ]);
});
