REQUIRE('chlk.templates.people.User');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('chlk.models.student.Summary');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.Summary*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/SummaryView.jade')],
        [ria.templates.ModelBind(chlk.models.student.Summary)],
        'Summary', EXTENDS(chlk.templates.people.User), [
            [ria.templates.ModelPropertyBind],
            Number, 'gradeLevelNumber',

            [ria.templates.ModelPropertyBind],
            String, 'currentClassName',

            [ria.templates.ModelPropertyBind],
            Number, 'currentAttendanceType',

            [ria.templates.ModelPropertyBind('currentAttendanceType', chlk.converters.attendance.AttendanceTypeToNameConverter)],
            String, 'attendanceTypeName',

            [ria.templates.ModelPropertyBind],
            Number, 'maxPeriodNumber',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.RoomId, 'roomId',

            [ria.templates.ModelPropertyBind],
            String, 'roomName',

            [ria.templates.ModelPropertyBind],
            Number, 'roomNumber',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.AttendanceHoverBox, 'attendanceBox',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.DisciplineHoverBox, 'disciplineBox',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.classes.Class), 'classesSection',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'periodSection'
        ])
});