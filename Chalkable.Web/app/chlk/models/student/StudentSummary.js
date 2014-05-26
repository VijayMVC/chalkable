REQUIRE('chlk.models.people.PersonSummary');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.announcement.AnnouncementClassPeriod');

REQUIRE('chlk.models.common.AttendanceHoverBoxItem');
REQUIRE('chlk.models.common.DisciplineHoverBoxItem');
REQUIRE('chlk.models.student.StudentGradesHoverBoxItem');
REQUIRE('chlk.models.student.StudentRankHoverBoxItem');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.StudentSummary*/
    CLASS(
        'StudentSummary', EXTENDS(chlk.models.people.PersonSummary), [
            [ria.serialize.SerializeProperty('gradelevelnumber')],
            Number, 'gradeLevelNumber',

            [ria.serialize.SerializeProperty('currentclassname')],
            String, 'currentClassName',

            [ria.serialize.SerializeProperty('currentattendancetype')],
            Number, 'currentAttendanceType',

            [ria.serialize.SerializeProperty('maxPeriodNumber')],
            Number, 'maxPeriodNumber',


            [ria.serialize.SerializeProperty('attendancebox')],
            chlk.models.common.HoverBox.OF(chlk.models.common.AttendanceHoverBoxItem), 'attendanceBox',

            [ria.serialize.SerializeProperty('disciplinebox')],
            chlk.models.common.HoverBox.OF(chlk.models.common.DisciplineHoverBoxItem), 'disciplineBox',

            [ria.serialize.SerializeProperty('gradesbox')],
            chlk.models.common.HoverBox.OF(chlk.models.student.StudentGradesHoverBoxItem), 'gradesBox',

            [ria.serialize.SerializeProperty('ranksbox')],
            chlk.models.common.HoverBox.OF(chlk.models.student.StudentRankHoverBoxItem), 'rankBox',

            [ria.serialize.SerializeProperty('classessection')],
            ArrayOf(chlk.models.classes.Class), 'classesSection',

            [ria.serialize.SerializeProperty('periodsection')],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'periodSection'
        ]);
});
