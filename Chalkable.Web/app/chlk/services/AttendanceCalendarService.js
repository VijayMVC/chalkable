REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AttendanceCalendarService */
    CLASS(
        'AttendanceCalendarService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getStudentAttendancePerMonth(studentId, date_) {
                return this.get('AttendanceCalendar/MonthForPerson.json', ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem) , {
                    date: date_ && date_.toString('mm-dd-yy'),
                    personId: studentId && studentId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getClassAttendancePerMonth(classId_, date_) {
                return this.get('AttendanceCalendar/MonthForClass.json', ArrayOf(chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem) , {
                    date: date_ && date_.toString('mm-dd-yy'),
                    classId: classId_ && classId_.valueOf()
                });
            }
        ])
});