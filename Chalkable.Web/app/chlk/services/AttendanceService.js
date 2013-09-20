REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.attendance.AttendanceSummary');
REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.attendance.AdminAttendanceSummary');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.id.MarkingPeriodId');
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
                    type: chlk.models.attendance.AttendanceTypeEnum.PRESENT.valueOf(),
                    date: date && date.toString('mm-dd-yy')
                });
            },

            [[Boolean, Boolean, Boolean, String, chlk.models.common.ChlkDate, chlk.models.id.MarkingPeriodId,
                chlk.models.id.MarkingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            ria.async.Future, function getAdminAttendanceSummary(renderNow, renderDay, renderMp, gradeLevelsIds_,
                nowDateTime_, fromMarkingPeriodId, toMarkingPeriodId, startDate_, endDate_) {
                    return this.get('Attendance/AdminAttendanceSummary.json', chlk.models.attendance.AdminAttendanceSummary, {
                        renderNow: renderNow || false,
                        renderDay: renderDay || false,
                        renderMp: renderMp || false,
                        gradeLevelsIds_: gradeLevelsIds_,
                        fromMarkingPeriodId: fromMarkingPeriodId.valueOf(),
                        toMarkingPeriodId: toMarkingPeriodId.valueOf(),
                        nowDateTime: nowDateTime_ && nowDateTime_.toString('mm-dd-yy'),
                        startDate: startDate_ && startDate_.toString('mm-dd-yy'),
                        endDate: endDate_ && endDate_.toString('mm-dd-yy')
                    });
            }
        ])
});