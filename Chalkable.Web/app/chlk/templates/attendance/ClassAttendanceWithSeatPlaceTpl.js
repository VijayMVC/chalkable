REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassAttendanceWithSeatPlaceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassAttendanceWithSeatPlace.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendance)],
        'ClassAttendanceWithSeatPlaceTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassAttendanceId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'type',

            [ria.templates.ModelPropertyBind],
            String, 'level',

            [ria.templates.ModelPropertyBind],
             chlk.models.people.User, 'student',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [ria.templates.ModelPropertyBind],
            String, 'attendanceReasonDescription',

            Boolean, 'smallPicture'
        ])
});