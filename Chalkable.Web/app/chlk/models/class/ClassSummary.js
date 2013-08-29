REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.HoverBox');
REQUIRE('chlk.models.announcement.AnnouncementsByDate');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassSummary*/
    CLASS(
        'ClassSummary', [
            String, 'room',

            ArrayOf(chlk.models.people.User), 'students',

            [ria.serialize.SerializeProperty('classsize')],
            Number, 'classSize',

            [ria.serialize.SerializeProperty('classattendancebox')],
            chlk.models.common.HoverBox, 'classAttendanceBox',

            [ria.serialize.SerializeProperty('classdisciplinebox')],
            chlk.models.common.HoverBox, 'classDisciplineBox',

            [ria.serialize.SerializeProperty('classattendancebox')],
            chlk.models.common.HoverBox, 'classAttendanceBox',

            [ria.serialize.SerializeProperty('announcementsbydate')],
            ArrayOf(chlk.models.announcement.AnnouncementsByDate), 'announcementsbydate',

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
