REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.attendance.AttendanceSummary');
REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.attendance.AdminAttendanceSummary');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.attendance.StudentDayAttendances');

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

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getStudentAttendance(studentId, datetime_) {
                return this.get('Attendance/GetAttendanceForStudent.json', chlk.models.attendance.StudentDayAttendances, {
                    studentId: studentId.valueOf(),
                    datetime: datetime_ && datetime_.toString('mm-dd-yy')
                });
            },

            [[String, String, String, String, chlk.models.common.ChlkDate]],
            ria.async.Future, function setAttendanceForList(classPersonIds, classPeriodIds, attendanceTypes, attReasons, date_) {
                return this.get('Attendance/SetAttendanceForList.json', Boolean, {
                    classPersonIds: classPersonIds,
                    classPeriodIds: classPeriodIds,
                    attendanceTypes: attendanceTypes,
                    attReasons: attReasons,
                    date: date_ && date_.toString('mm-dd-yy')
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId, String, chlk.models.id.AttendanceReasonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function setAttendance(studentId, classId, level, attendanceReasonId_, date) {
                return this.get('Attendance/SetAttendance.json', Boolean, {
                    personId: studentId.valueOf(),
                    classId: classId.valueOf(),
                    attendanceReasonId: attendanceReasonId_ && attendanceReasonId_.valueOf(),
                    level: level,
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
                        gradeLevelsIds: gradeLevelsIds_,
                        fromMarkingPeriodId: fromMarkingPeriodId.valueOf(),
                        toMarkingPeriodId: toMarkingPeriodId.valueOf(),
                        nowDateTime: nowDateTime_ && nowDateTime_.toString('mm-dd-yy'),
                        startDate: startDate_ && startDate_.toString('mm-dd-yy'),
                        endDate: endDate_ && endDate_.toString('mm-dd-yy')
                    });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.MarkingPeriodId]],
            ria.async.Future, function getStudentAttendanceSummary(studentId, markingPeriodId){
                return this.get('Attendance/StudentAttendanceSummary.json', chlk.models.attendance.StudentAttendanceSummary,{
                    personId: studentId && studentId.valueOf(),
                    markingPeriodId: markingPeriodId && markingPeriodId.valueOf()
                });
            }
        ])
});