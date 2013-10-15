REQUIRE('chlk.models.people.PersonSummary');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.common.AttendanceHoverBox');
REQUIRE('chlk.models.common.DisciplineHoverBox');
REQUIRE('chlk.models.announcement.AnnouncementClassPeriod');
REQUIRE('chlk.models.student.StudentRankHoverBox');
REQUIRE('chlk.models.student.StudentGradesHoverBox');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.Summary*/
    //todo rename this class to StudentSummary
    CLASS(
        'Summary', EXTENDS(chlk.models.people.PersonSummary), [
            [ria.serialize.SerializeProperty('gradelevelnumber')],
            Number, 'gradeLevelNumber',

            [ria.serialize.SerializeProperty('currentclassname')],
            String, 'currentClassName',

            [ria.serialize.SerializeProperty('currentattendancetype')],
            Number, 'currentAttendanceType',

            [ria.serialize.SerializeProperty('maxPeriodNumber')],
            Number, 'maxPeriodNumber',


            [ria.serialize.SerializeProperty('attendancebox')],
            chlk.models.common.AttendanceHoverBox, 'attendanceBox',

            [ria.serialize.SerializeProperty('disciplinebox')],
            chlk.models.common.DisciplineHoverBox, 'disciplineBox',

            [ria.serialize.SerializeProperty('gradesbox')],
            chlk.models.student.StudentGradesHoverBox, 'gradesBox',

            [ria.serialize.SerializeProperty('ranksbox')],
            chlk.models.student.StudentRankHoverBox, 'rankBox',

            [ria.serialize.SerializeProperty('classessection')],
            ArrayOf(chlk.models.classes.Class), 'classesSection',

            [ria.serialize.SerializeProperty('periodsection')],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'periodSection'
        ]);
});
