REQUIRE('chlk.models.calendar.BaseMonthCalendar');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.calendar', function(){
    "use strict";

    /**@class chlk.models.calendar.StudentProfileMonthCalendar*/
    CLASS('StudentProfileMonthCalendar', EXTENDS(chlk.models.calendar.BaseMonthCalendar),[

        chlk.models.id.SchoolPersonId, 'studentId',
        chlk.models.common.ChlkDate, 'minDate',
        chlk.models.common.ChlkDate, 'maxDate',

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
        function $(date_, minDate_, maxDate_, studentId){
            BASE(date_, minDate_, maxDate_);
            if(minDate_)
                this.setMinDate(minDate_);
            if(maxDate_)
                this.setMaxDate(maxDate_);
            if(studentId)
                this.setStudentId(studentId);
        }
    ]);
});