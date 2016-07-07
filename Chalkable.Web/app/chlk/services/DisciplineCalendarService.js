REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DisciplineCalendarService */
    CLASS(
        'DisciplineCalendarService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getStudentDisciplinePerMonth(studentId, date_) {
                return this.get('DisciplineCalendar/MonthForStudent.json',
                    ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem) , {

                        date: date_ && date_.toString('mm-dd-yy'),
                        studentId: studentId && studentId.valueOf()
                });
            }
        ]);
});