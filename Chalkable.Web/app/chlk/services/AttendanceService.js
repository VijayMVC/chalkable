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
REQUIRE('chlk.models.attendance.SetClassListAttendance');
REQUIRE('chlk.models.attendance.SeatingChart');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AttendanceService */
    CLASS(
        'AttendanceService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getSummary() {
                return this.get('Attendance/AttendanceSummary.json', chlk.models.attendance.AttendanceSummary, {});
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
            ria.async.Future, function getClassList(classId, date_, byLastName_) {
                return this.get('Attendance/ClassList.json', ArrayOf(chlk.models.attendance.ClassAttendance), {
                    classId: classId.valueOf(),
                    date: date_ && date_.toStandardFormat(),
                    byLastName: byLastName_
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getSeatingChartInfo(classId, date_) {
                return this.get('Attendance/SeatingChart.json', chlk.models.attendance.SeatingChart, {
                    classId: classId.valueOf(),
                    date: date_ && date_.toStandardFormat()
                });
            },

            [[chlk.models.id.ClassId, Number, Number]],
            ria.async.Future, function updateSeatingChart(classId, columns, rows) {
                return this.get('Attendance/UpdateSeatingChart.json', chlk.models.attendance.SeatingChart, {
                    classId: classId.valueOf(),
                    columns: columns,
                    rows: rows
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.SchoolPersonId, Number]],
            ria.async.Future, function changeStudentSeat(classId, studentId, index) {
                return this.get('Attendance/ChangeStudentSeat.json', chlk.models.attendance.SeatingChart, {
                    classId: classId.valueOf(),
                    studentId: studentId,
                    index: index
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getStudentAttendance(studentId, datetime_) {
                return this.get('Attendance/GetAttendanceForStudent.json', chlk.models.attendance.StudentDayAttendances, {
                    studentId: studentId.valueOf(),
                    datetime: datetime_ && datetime_.toStandardFormat()
                });
            },

            [[chlk.models.id.SchoolPersonId, String, String, String, chlk.models.common.ChlkDate]],
            ria.async.Future, function setAttendanceForList(personId, classIds, attendanceTypes, attReasons, date_) {
                return this.get('Attendance/SetAttendanceForList.json', Boolean, {
                    personId: personId.valueOf(),
                    classIds: classIds,
                    levels: attendanceTypes,
                    attReasons: attReasons,
                    date: date_ && date_.toStandardFormat()
                });
            },

            [[chlk.models.attendance.SetClassListAttendance]],
            ria.async.Future, function setAttendance(setAttendanceData) {
                return this.post('Attendance/SetAttendance.json', Boolean,{
                    classId: setAttendanceData.getClassId() && setAttendanceData.getClassId().valueOf(),
                    date: setAttendanceData.getDate().toStandardFormat(),
                    items: setAttendanceData.getPostItems()
                });
            },

            [[chlk.models.common.ChlkDate, Object]],
            ria.async.Future, function postSeatingChart(date, postSeatingChartData) {
                return this.post('Attendance/PostSeatingChart.json', Boolean,{
                    seatingChartInfo: postSeatingChartData,
                    date: date.toStandardFormat(),
                    needInfo: false
                });
            },

            [[chlk.models.common.ChlkDate, Object]],
            ria.async.Future, function postSeatingChartWithInfo(date, postSeatingChartData) {
                return this.post('Attendance/PostSeatingChart.json', chlk.models.attendance.SeatingChart,{
                    seatingChartInfo: postSeatingChartData,
                    date: date.toStandardFormat(),
                    needInfo: true
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function markAllPresent(classId, date) {
                var level =  new chlk.models.attendance.AttendanceTypeMapper()
                    .mapBack(chlk.models.attendance.AttendanceTypeEnum.PRESENT);

                return this.get('Attendance/SetAttendanceForClass.json', Boolean, {
                    classId: classId && classId.valueOf(),
                    level: level,
                    date: date && date.toStandardFormat()
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
                        nowDateTime: nowDateTime_ && nowDateTime_.toStandardFormat(),
                        startDate: startDate_ && startDate_.toStandardFormat(),
                        endDate: endDate_ && endDate_.toStandardFormat()
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