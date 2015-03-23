REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassAttendanceWithSeatPlaceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassAttendanceWithSeatPlace.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendance)],
        'ClassAttendanceWithSeatPlaceTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            chlk.models.id.ClassAttendanceId, 'id',

            chlk.models.common.ChlkDate, 'date',

            Number, 'type',

            READONLY, String, 'typeName',

            String, 'level',

            chlk.models.people.User, 'student',

            chlk.models.id.SchoolPersonId, 'studentId',

            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            String, 'className',

            chlk.models.id.ClassId, 'classId',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            String, 'attendanceReasonDescription',

            Boolean, 'smallPicture',

            OVERRIDE, VOID, function assign(model) {
                this.model = model;
                this.id = model.getId();
                this.date = model.getDate();
                this.type = model.getType();
                this.typeName = model.getTypeName();
                this.student = model.getStudent();
                this.studentId = model.getStudentId();
                this.attendanceReasonId = model.getAttendanceReasonId();
                this.attendanceReason = model.getAttendanceReason();
                this.className = model.getClassName();
                this.classId = model.getClassId();
                this.reasons = model.getReasons();
                this.attendanceReasonDescription = model.getAttendanceReasonDescription();
            }
        ])
});