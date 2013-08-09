REQUIRE('chlk.models.class.ClassesForTopBar');
REQUIRE('chlk.models.calendar.TeacherSettingsCalendarDay');
REQUIRE('chlk.models.grading.Final');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.TeacherSettings*/
    CLASS(
        'TeacherSettings', [
            chlk.models.class.ClassesForTopBar, 'topData',
            ArrayOf(chlk.models.calendar.TeacherSettingsCalendarDay), 'calendarInfo',
            chlk.models.grading.Final, 'gradingInfo'
        ]);
});
