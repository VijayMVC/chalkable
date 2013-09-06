REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.HoverBox');
REQUIRE('chlk.models.common.CommonHoverBox');
REQUIRE('chlk.models.common.DisciplineHoverBox');
REQUIRE('chlk.models.classes.Room');
REQUIRE('chlk.models.announcement.AnnouncementsByDate');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassSummary*/
    CLASS(
        'ClassSummary', [
            chlk.models.classes.Room, 'room',

            ArrayOf(chlk.models.people.User), 'students',

            [ria.serialize.SerializeProperty('classsize')],
            Number, 'classSize',

            [ria.serialize.SerializeProperty('classattendancebox')],
            chlk.models.common.CommonHoverBox, 'classAttendanceBox',

            [ria.serialize.SerializeProperty('classdisciplinebox')],
            chlk.models.common.DisciplineHoverBox, 'classDisciplineBox',

            [ria.serialize.SerializeProperty('classaveragebox')],
            chlk.models.common.CommonHoverBox, 'classAverageBox',

            [ria.serialize.SerializeProperty('announcementsbydate')],
            ArrayOf(chlk.models.announcement.AnnouncementsByDate), 'announcementsByDate',

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
