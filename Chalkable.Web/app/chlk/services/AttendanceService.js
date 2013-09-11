REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.attendance.AttendanceSummary');
REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AttendanceService */
    CLASS(
        'AttendanceService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getSummary() {
                return this.get('Attendance/AttendanceSummary.json', chlk.models.attendance.AttendanceSummary, {});
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getClassList(classId, date_) {
                return this.get('Attendance/ClassList.json', ArrayOf(chlk.models.attendance.ClassAttendance), {
                    classId: classId.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy')
                });
            },

            [[chlk.models.id.ClassPersonId, chlk.models.id.ClassPeriodId, Number, chlk.models.id.AttendanceReasonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function setAttendance(classPersonId, classPeriodId, type, attendanceReasonId_, date) {
                return this.get('Attendance/SetAttendance.json', Boolean, {
                    classPersonId: classPersonId.valueOf(),
                    classPeriodId: classPeriodId.valueOf(),
                    attendanceReasonId: attendanceReasonId_ && attendanceReasonId_.valueOf(),
                    type: type,
                    date: date && date.toString('mm-dd-yy')
                });
            },

            [[chlk.models.id.ClassPeriodId, chlk.models.common.ChlkDate]],
            ria.async.Future, function markAllPresent(classPeriodId, date) {
                return this.get('Attendance/SetAttendanceForClass.json', Boolean, {
                    classPeriodId: classPeriodId.valueOf(),
                    type: chlk.models.attendance.AttendanceTypesValue.PRESENT.valueOf(),
                    date: date && date.toString('mm-dd-yy')
                });
            }
        ])
});