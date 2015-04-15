REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.period.ClassPeriod');

NAMESPACE('chlk.models.calendar.announcement', function(){
    "use strict";
    /**@class chlk.models.calendar.announcement.AdminDayCalendarItem*/
    CLASS('AdminDayCalendarItem', [
        chlk.models.period.Period, 'period',

        [ria.serialize.SerializeProperty('gradelevel')],
        chlk.models.grading.GradeLevel, 'gradeLevel',

        [ria.serialize.SerializeProperty('classperiods')],
        ArrayOf(chlk.models.period.ClassPeriod), 'classPeriods'



    ]);
});
